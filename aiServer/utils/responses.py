#----------------------------------------------------------------------
#  responses.py
#
# The Voice : Response util methods.
# Author :  Korhan Akcura
#----------------------------------------------------------------------
import re, string

class responses:
	def __init__(self):
		self.punctuation_regex = re.compile('[%s]' % re.escape(string.punctuation))
		self.end_punctuations = [".", "?", "!"]

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
			#self.get_last_punctuation_location(response)

		return response

	def get_last_punctuation_location(self,response):
		last_end_punctuation_index = 0
		#for punctuation in self.end_punctuations:
		#	if last_end_punctuation_index



		location = 0

		location = self.punctuation_regex.search(response[::-1]).end()

		print(location)

		return location
