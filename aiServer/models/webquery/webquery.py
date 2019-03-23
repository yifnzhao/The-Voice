#----------------------------------------------------------------------
#  webquery.py
#
# The Voice : Query web for answers.
# Author :  Korhan Akcura
#----------------------------------------------------------------------
import time
from utils import responses
from . import duckduckgo_facade
from timeout3 import timeout, TIMEOUT_EXCEPTION

class webquery:
	def __init__(self):
		self.response_util= responses.responses()
		self.duckduckgo_api = duckduckgo_facade.duckduckgo_facade()

	#----------------------------------------------------------------------
	#  Get answer from web.
	#----------------------------------------------------------------------
	@timeout(10)
	def respond(self,str):
		# Get DuckDuckGo response.
		response = self.duckduckgo_api.respond(str)
		if response == "":
			return response
		return self.response_util.clean_html(response)