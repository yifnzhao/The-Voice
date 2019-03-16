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
import time
from timeout3 import timeout, TIMEOUT_EXCEPTION
from utils import database
from chatterbot import ChatBot
from chatterbot.trainers import ChatterBotCorpusTrainer
from chatterbot.trainers import ListTrainer

class chatterbot_facade:
	def __init__(self):
		self.chatbot = ""
		self.trainer = ""
		self.logic_adapters="chatterbot.logic.BestMatch"
		self.maximum_similarity_threshold = 0.90
		self.statement_comparison_function = "chatterbot.comparisons.levenshtein_distance"

		self.dbname = "learnbot"
		self.db_util= database.database()
		self.isMongoDB = self.db_util.check_db_exists()

		if self.isMongoDB:
			print("Using the found MongoDB instance.")
			self.initilize_mongo()
			#if not self.db_util.check_mongo_db_exists(self.dbname):
			self.initial_traning()

		else:
			print("No MongoDB instance found.")
			self.initilize_no_mongo()

	def initilize_mongo(self):
		self.chatbot = ChatBot("LearnBot",
								storage_adapter='chatterbot.storage.MongoDatabaseAdapter',
								database_uri=self.db_util.get_connection_url() + "/" + self.dbname,
								database=self.dbname,
								logic_adapters=[
									'chatterbot.logic.MathematicalEvaluation',
									'chatterbot.logic.TimeLogicAdapter',
									'chatterbot.logic.BestMatch'
								]
							)

	def initilize_no_mongo(self):
		self.chatbot = ChatBot("LearnBot",
								storage_adapter="chatterbot.storage.SQLStorageAdapter",
								logic_adapters=[
									{
										"import_path": self.logic_adapters,
										"default_response": "",
										"maximum_similarity_threshold": self.maximum_similarity_threshold,
										"statement_comparison_function": self.statement_comparison_function
									}
								],
								filters=[
									"chatterbot.filters.RepetitiveResponseFilter"
								]
							)
		self.initial_traning()


	def initial_traning(self):
		print("Training...")
		trainer = ChatterBotCorpusTrainer(self.chatbot)
		trainer.train("chatterbot.corpus.english")
		self.trainer = ListTrainer(self.chatbot)
		self.trainer.train([
			"How are you?",
			"I am good.",
			"That is good to hear.",
			"Thank you",
			"You are welcome.",
		])

	#----------------------------------------------------------------------
	#  Predict the emotion of a text as happy, sad or natural.
	#----------------------------------------------------------------------
	@timeout(5)
	def respond(self,str):
		response = self.chatbot.get_response(str, search_text=str).text
		#response = self.chatbot.get_response(str).text
		return response
