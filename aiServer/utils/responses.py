#----------------------------------------------------------------------
#  responses.py
#
# The Voice : Response util methods.
# Author :  Korhan Akcura
#----------------------------------------------------------------------
import re, string

class responses:
	def __init__(self):
		self.end_punctuations = ".?!"
		self.punctuation_regex = re.compile('[%s]' % re.escape(self.end_punctuations))

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
			response = self.get_first_sentence(response)

		return response

	def get_first_sentence(self,response):
		sentence_array = self.punctuation_regex.split(response)
		first_sentence = sentence_array[:1][0] + "."
		# If it is too short try to add more.
		if len(first_sentence) < 10:
			try:
				first_sentence += sentence_array[:2][0] + "."
			except:
				pass
		return first_sentence

	def clean_dynamic(self, string):
		return self.remove_non_end_punctuation(self.clean_html(string)).strip()

	def clean_html(self, string_with_html):
		cleanr = re.compile('<.*?>')
		cleantext = re.sub(cleanr, '', string_with_html)
		return cleantext

	def remove_non_end_punctuation(self, string):
		non_end_punctuations = '''()-[]{};:'"\,<>/@#$%^&*_~'''
		for char in string: 
			if char in non_end_punctuations: 
				cleantext = string.replace(char, "")
		return cleantext
