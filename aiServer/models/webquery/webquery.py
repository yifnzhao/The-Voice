#----------------------------------------------------------------------
#  webquery.py
#
# The Voice : Query web for answers.
# Author :  Korhan Akcura
#----------------------------------------------------------------------
import time
from utils import responses
from . import duckduckgo_facade
from . import response_scraper
from timeout3 import timeout, TIMEOUT_EXCEPTION

class webquery:
	def __init__(self):
		self.response_util= responses.responses()
		self.duckduckgo_api = duckduckgo_facade.duckduckgo_facade()
		self.response_scraper = response_scraper.response_scraper()

	#----------------------------------------------------------------------
	#  Get answer from web.
	#----------------------------------------------------------------------
	@timeout(5)
	def respond(self,str):
		# Get DuckDuckGo response.
		response = self.duckduckgo_api.respond(str)
		if response == "":
			response = self.response_scraper.respond(str)
		return self.response_util.clean_dynamic(response)