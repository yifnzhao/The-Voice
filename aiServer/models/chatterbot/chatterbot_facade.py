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
from chatterbot.storage import StorageAdapter
from chatterbot.trainers import ChatterBotCorpusTrainer
from chatterbot.trainers import UbuntuCorpusTrainer
from chatterbot.trainers import ListTrainer

class chatterbot_facade:
	def __init__(self):
		self.chatbot = ""
		self.trainer = ""
		self.logic_adapter_1="chatterbot.logic.BestMatch"
		self.maximum_similarity_threshold =  0.95
		self.statement_comparison_function = "chatterbot.comparisons.LevenshteinDistance"

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
										"import_path": self.logic_adapter_1,
										"default_response": "",
										"maximum_similarity_threshold": self.maximum_similarity_threshold,
										"statement_comparison_function": self.statement_comparison_function
									}
								],
								filters=[
									"chatterbot.filters.RepetitiveResponseFilter"
								],
								read_only=True
							)		

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

	def additional_traning(self):
		print("Additional Training...")
		print("This will take a while!!!")
		trainer = UbuntuCorpusTrainer(self.chatbot)
		trainer.train()


	def train(self,query,response):
		print("LearnBot Training...")
		if self.trainer == "":
			self.trainer = ListTrainer(self.chatbot)
		#self.chatbot.learn_response(response, query)
		self.trainer.train([query, response])	

	#----------------------------------------------------------------------
	#  Predict the emotion of a text as happy, sad or natural.
	#----------------------------------------------------------------------
	@timeout(5)
	def respond(self,str):
		response = self.chatbot.get_response(str, search_text=str)
		#print(response.text)
		#print(response.confidence)
		if response.confidence < self.maximum_similarity_threshold:
			return ""
		#response = self.chatbot.get_response(str).text
		return response.text
