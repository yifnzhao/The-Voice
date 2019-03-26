#----------------------------------------------------------------------
#  response_scraper.py
#
# Get response from Web without an API.
# For now only uses Yahoo Answers.
# Authors :  Korhan Akcura
#----------------------------------------------------------------------
import requests
from bs4 import BeautifulSoup
from timeout3 import timeout, TIMEOUT_EXCEPTION

class response_scraper:
	def __init__(self):
		self.yahoo_answers_search_url = 'https://ca.answers.search.yahoo.com/search?p='
		self.yahoo_answers_responses_url = 'https://ca.answers.yahoo.com/question/index'
		
	#----------------------------------------------------------------------
	#  Scrape a response from Web.
	#----------------------------------------------------------------------
	@timeout(3)
	def respond(self,query):
		query_url = self.yahoo_answers_search_url+query;
		
		response = requests.get(query_url)
		query_page = str(BeautifulSoup(response.content.decode('utf-8','ignore'), features="lxml"))

		response_url = self.getResponseUrl(query_page)
		response = requests.get(response_url)
		response_page = str(BeautifulSoup(response.content.decode('utf-8','ignore'), features="lxml"))
		
		return self.getResponse(response_page)

	def getResponseUrl(self,page):
		start_link = page.find('href="'+self.yahoo_answers_responses_url)
		if start_link == -1:
			return ""
		start_quote = page.find('"', start_link)
		end_quote = page.find('"', start_quote + 1)
		response_url = page[start_quote + 1: end_quote]
		return response_url

	def getResponse(self,page):
		start_response = page.find('Best Answer')
		if start_response == -1:
			return ""
		start_quote = page.find('>',page.find('<span', start_response))
		end_quote = page.find('</span>', start_quote)
		if start_quote == -1 or end_quote == -1:
			return ""
		response = page[start_quote + 1: end_quote]
		return response
