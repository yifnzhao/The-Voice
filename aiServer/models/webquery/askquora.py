#----------------------------------------------------------------------
#  askquora.py
#
# The implementation contains code from;
# https://github.com/ritiek/AskQuora/blob/master/askquora/askquora.py - MIT License
# !!Not Working as expected so not using it!!
#----------------------------------------------------------------------

from colorama import init, Fore, Style
from bs4 import BeautifulSoup
import os
import sys
import random
import requests
import textwrap
import argparse

class askquora:
	def __init__(self):
		# selecting a random header gives more time before rate limiting
		self.HEADERS = (
			'Mozilla/5.0 (Macintosh; Intel Mac OS X 10.7; rv:11.0) Gecko/20100101 Firefox/11.0',
			'Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:22.0) Gecko/20100 101 Firefox/22.0',
			'Mozilla/5.0 (Windows NT 6.1; rv:11.0) Gecko/20100101 Firefox/11.0',
			'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_4) AppleWebKit/536.5 (KHTML, like Gecko)',
			'Chrome/19.0.1084.46 Safari/536.5',
			'Mozilla/5.0 (Windows; Windows NT 6.1) AppleWebKit/536.5 (KHTML, like Gecko)',
			'Chrome/19.0.1084.46', 'Safari/536.5',
			'Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9.2.13) Gecko/20101206 Ubuntu/10.10 (maverick) Firefox/3.6.13'
		)

	# receive user selected question
	def get_input_link(self,links):
		selection = 1
		return links[selection - 1]


	# duckduckgo links are different from actual webpage links
	def duckduckgo_links(self,query):
		header = {'User-agent': random.choice(self.HEADERS)}

		page = requests.get(
			'https://duckduckgo.com/html/?q=' + query + ' site:quora.com',
			headers=header).text
		soup = BeautifulSoup(page, 'html.parser')

		possible_links = soup.find_all('a', {'class': 'result__a'})

		return possible_links


	# decode a duckduckgo link to actual webpage link
	def decode_result(self,link):
		header = {'User-agent': random.choice(self.HEADERS)}
		inner_link = 'https://duckduckgo.com' + link['href']

		page = requests.get(inner_link, headers=header).text
		soup = BeautifulSoup(page, 'html.parser')

		link = (soup.find('script').get_text()).replace(
			'window.parent.location.replace("', '').replace('");', '')

		return link


	# show title of the question
	def show_title(self,link, numb, color=None):
		width = int((os.popen('stty size', 'r').read().split())[1])

		if color is None:
			Fore.RED = ''
			Fore.MAGENTA = ''
			Style.BRIGHT = ''

		if color:
			prefix = Fore.RED + Style.BRIGHT + '{0: <4}'.format(str(numb) + '.')
		else:
			prefix = Fore.MAGENTA + Style.BRIGHT + '{0: <4}'.format(
				str(numb) + '.')

		wrapper = textwrap.TextWrapper(
			initial_indent=prefix, width=width, subsequent_indent='    ')

		title = wrapper.fill(link.replace('https://www.quora.com/', ''))
		readable_title = title.replace('?share=1', '').replace('-', ' ') + '?'
		return readable_title


	# decode all links and display title of all questions
	def correct_links(self,possible_results, colored):
		links = []
		numb = 1
		color = False

		titles = []
		for result in possible_results[:10]:
			link = self.decode_result(result)
			if self.is_question(link):
				if colored:
					titles.append(self.show_title(link, numb, color))
				else:
					titles.append(self.show_title(link, numb))
				links.append(link)

				color = not color
				numb += 1

		return links


	# check if the link is a quora question
	def is_question(self,link):
		if link.startswith('https://www.quora.com/') and not link.startswith(
				'https://www.quora.com/topic/') and not link.startswith(
					'https://www.quora.com/profile/'):
			return True
		else:
			return False


	# display answer to user chosen question
	def answer_question(self,link, colored=False):
		answer = ""

		# quora has a weird bug sometimes where it won't display the answer even if question is answered
		# trying multiple times help reduce chance of it affecting us in this case
		for headere in self.HEADERS:
			header = {'User-agent': headere}

			ques_page = requests.get(link, headers=header).text
			ques_page = ques_page.replace('<br />', '\n')
			ques_page = ques_page.replace('</p>', '\n\n')

			if colored:
				ques_page = ques_page.replace('<b>', Fore.YELLOW).replace(
					'</b>', Fore.RED)
				ques_page = ques_page.replace('<a', Fore.BLUE + '<a').replace(
					'</a>', Fore.RED + '</a>')

			soup = BeautifulSoup(ques_page, 'html.parser')

			try:
				answer = soup.find('div', {'class':
										   'ExpandedQText ExpandedAnswer'}).get_text()
				break

			except AttributeError:
				answer = ""
				continue

			finally:
				if colored:
					answer = Fore.RED + Style.BRIGHT + answer

		return answer


	# parse the query into a searchable query
	def generate_search_query(self,query):
		query = (' '.join(query)).replace(' ', '+')
		return query


	# the main function
	def respond(self,query):
		colored=False
		query = self.generate_search_query(query)

		encoded_links = self.duckduckgo_links(query)

		question_links = self.correct_links(encoded_links, colored)

		question_link = self.get_input_link(question_links)

		answer = self.answer_question(question_link)

		return answer
