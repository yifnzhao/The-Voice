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
from chatterbot import ChatBot
#from chatterbot.trainers import ChatterBotCorpusTrainer

class chatterbot_facade:
	def __init__(self):
		self.chatbot = ChatBot('LearnBot')
		#trainer = ChatterBotCorpusTrainer(self.chatbot)
		#trainer.train("chatterbot.corpus.english")

	#----------------------------------------------------------------------
	#  Predict the emotion of a text as happy, sad or natural.
	#----------------------------------------------------------------------
	@timeout(5)
	def respond(self,str):
		response = self.chatbot.get_response(str, search_text=str).text
		#response = self.chatbot.get_response(str).text
		return response
