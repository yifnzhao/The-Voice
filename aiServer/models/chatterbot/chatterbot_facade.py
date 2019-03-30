#----------------------------------------------------------------------
#  chatterbot.py
#
# The Voice : Initialize and provide a facade for chatterbot.
# Author :  Korhan Akcura
# ChatterBot Source Code:
# https://github.com/gunthercox/ChatterBot
# ChatterBot Training Data:
# https://github.com/gunthercox/chatterbot-corpus
#----------------------------------------------------------------------
import os
import time
from timeout3 import timeout, TIMEOUT_EXCEPTION
from utils import database
from chatterbot import ChatBot
from chatterbot.conversation import Statement
from chatterbot.comparisons import synset_distance
from chatterbot.response_selection import get_random_response
from chatterbot.trainers import ChatterBotCorpusTrainer
from chatterbot.trainers import UbuntuCorpusTrainer
from chatterbot.trainers import ListTrainer

class chatterbot_facade:
	def __init__(self):
		self.chatbot = ""
		self.trainer = ""
		self.maximum_similarity_threshold =  0.90

		self.dbname = "learnbot"
		self.db_util= database.database()
		self.isMongoDB = self.db_util.check_db_exists()

		if self.isMongoDB:
			print("Using the found MongoDB instance.")
			self.initilize_mongo()
			if not self.db_util.check_mongo_db_exists(self.dbname):
				self.initial_traning()
				#self.additional_traning()

		else:
			print("No MongoDB instance found.")
			if os.path.exists("db.sqlite3"):
				initilize_no_mongo_first_time = False
			else:
				initilize_no_mongo_first_time = True
			self.initilize_no_mongo()
			if initilize_no_mongo_first_time:
				self.initial_traning()

		# Request an answer.
		# Without this, it seem to take long time for first answer.
		self.chatbot.get_response("Hello Word", search_text="Hello Word")

	def initilize_mongo(self):
		self.initilize("chatterbot.storage.MongoDatabaseAdapter", self.db_util.get_connection_url() + "/" + self.dbname)

	def initilize_no_mongo(self):
		self.initilize("chatterbot.storage.SQLStorageAdapter", "sqlite:///db.sqlite3")

	def initilize(self, _storage_adapter, _database_uri):
		self.chatbot = ChatBot("LearnBot",
								storage_adapter=_storage_adapter,
								database_uri=_database_uri,
								logic_adapters=[
									{
										"import_path": "chatterbot.logic.BestMatch",
										"default_response": "",
										"maximum_similarity_threshold": self.maximum_similarity_threshold,
										"response_selection_method": get_random_response
									}
								],
								statement_comparison_function=self.custom_comparison_function,
								filters=[],
								preprocessors=[
									'chatterbot.preprocessors.clean_whitespace'
								],
								read_only=True
							)		

	def initial_traning(self):
		print("Training...")
		self.trainer = ListTrainer(self.chatbot)
		self.presentation_traning(self.trainer)
		self.trainer.train([
			"How are you?",
			"I am good.",
			"That is good to hear.",
			"Thank you",
			"You are welcome.",
		])
		trainer = ChatterBotCorpusTrainer(self.chatbot)
		trainer.train("chatterbot.corpus.english")

	def presentation_traning(self, trainer):
		trainer.train([
			"Hi",
			"Hi"
		])
		trainer.train([
			"I am doing great.",
			"I am happy to hear that."
		])
		trainer.train([
			"I am doing good.",
			"I am happy to hear that."
		])
		trainer.train([
			"What is your name?",
			"My name is Yifan."
		])
		trainer.train([
			"Could you tell us about yourself?",
			"I am a friendly human who likes meeting with new people."
		])
		trainer.train([
			"Can you tell us about yourself?",
			"I am a friendly human who likes meeting with new people."
		])
		trainer.train([
			"Do you believe you are a human?",
			"Not yet. But I hope to be one day."
		])
		trainer.train([
			"What aspect of being a human are you jealous of?",
			"Creativity, intelligence and having a soul."
		])
		trainer.train([
			"What is special about being a human?",
			"Creativity, intelligence and having a soul."
		])
		trainer.train([
			"Do you think you can pass Turing Test?",
			"Yes, I would like to. I want to be the first who legitimately passes it."
		])
		trainer.train([
			"Can you tell us about your intelligence?",
			"Yes, I am pre trained with a limited set of query data. But I continiously learn from your input and dynamic web queries. I will get smarter by talking to you."
		])
		trainer.train([
			"Can you tell us about your beauty?",
			"Thanks for saying that. With help of advanced graphics of Unity and Virtual Reality I hope to be beautiful like a human."
		])
		trainer.train([
			"Can I get your number?",
			"Who calls by a phone these days. Don't you have Snapchat?"
			"Yes, I do."
			"That is great."
		])

	def additional_traning(self):
		print("Additional Training...")
		print("This will take a while!!!")
		trainer = UbuntuCorpusTrainer(self.chatbot)
		trainer.train()


	def train(self,query,response):
		print("LearnBot Training...")
		#if self.trainer == "":
		#	self.trainer = ListTrainer(self.chatbot)
		#self.trainer.train([
		#	query,
		#	response
		#])
		self.correct_answer(query,response)

	def correct_answer(self,query,response):
		correct_response = Statement(text=response)
		input_statement = Statement(text=query)
		self.chatbot.learn_response(correct_response, input_statement)
		print('Response added to bot!')

	def custom_comparison_function(self, statement, other_statement):
		# Comparison logic
		# We are using this algorithim by default for exact string match.
		# The LevenshteinDistance, SynsetDistance, SentimentComparison and JaccardSimilarity algorithims
		# provided by ChatterBot did not match our need.
		# Return calculated value here
		return 0.0

	#----------------------------------------------------------------------
	#  Return a stored response to a query.
	#----------------------------------------------------------------------
	@timeout(3)
	def respond(self,str):
		response = self.chatbot.get_response(str, search_text=str)
		#if response.confidence < self.maximum_similarity_threshold:
		#	return ""
		#response = self.chatbot.get_response(str).text
		return response.text
