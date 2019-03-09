#----------------------------------------------------------------------
#  duckduckgo.py
#
# The Voice : Provide a facade for duckduckgo API.
# Author :  Korhan Akcura
#----------------------------------------------------------------------
from duckduckpy import query as duckduckquery
import time
from timeout3 import timeout, TIMEOUT_EXCEPTION

class duckduckgo_facade:

	#----------------------------------------------------------------------
	#  Query and get abstract as response from duckduckgo API.
	#----------------------------------------------------------------------
	@timeout(3)
	def respond(self,str):
		# Doing below doesn't always return revelant answers.
		# response = response.related_topics[0].text
		# This will return empty if there is no abstract text.
		response = duckduckquery(str).abstract_text
		return response