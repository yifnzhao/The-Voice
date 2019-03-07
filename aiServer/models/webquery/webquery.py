#----------------------------------------------------------------------
#  webquery.py
#
# The Voice : Query web for answers.
# Author :  Korhan Akcura
#----------------------------------------------------------------------
import time
from . import duckduckgo_facade
from timeout3 import timeout, TIMEOUT_EXCEPTION

class webquery:
	def __init__(self):
		self.duckduckgo_api = duckduckgo_facade.duckduckgo_facade()

	#----------------------------------------------------------------------
	#  Get answer from web.
	#----------------------------------------------------------------------
	@timeout(5)
	def respond(self,str):
		response = self.duckduckgo_api.respond(str)
		#response = self.chatbot.get_response(str).text
		print(response)
