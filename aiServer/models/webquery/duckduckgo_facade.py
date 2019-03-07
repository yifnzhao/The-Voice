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
		response = duckduckquery(str).related_topics[0]
		return response
