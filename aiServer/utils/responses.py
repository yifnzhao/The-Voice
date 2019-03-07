#----------------------------------------------------------------------
#  responses.py
#
# The Voice : Response util methods.
# Author :  Korhan Akcura
#----------------------------------------------------------------------

class responses:

	#----------------------------------------------------------------------
	#  Do the necessary formatting on the response.
	#----------------------------------------------------------------------
	def format_response(self,response):
		# Default prediction.

		# Shorten the response to the voice to an acceptable limit to voice server.
		formatted_response = self.shorten_response(response)

		return formatted_response

	def shorten_response(self,response):
		# Shorten the answer limit to 256 characters if longer.
		if len(response) > 256: 
			# Get first 256 characters.
			# We know just doing this might result with incomplate responses but it is ok for now.
			response = response[:256]

		return response
