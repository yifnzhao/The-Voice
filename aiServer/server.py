#----------------------------------------------------------------------
#  server.py
#
# The Voice : Main server code for NLP and text responses.
# Author :  Korhan Akcura
#----------------------------------------------------------------------
import json
import codecs
import requests
import logging
#logging.basicConfig(level=logging.CRITICAL)
from func_timeout import func_timeout
from utils import *
from models.eliza import eliza
from models.chatterbot import chatterbot_facade
from models.webquery import webquery
from stubs.emotion import emotion

from logging.handlers import RotatingFileHandler

from flask import Flask, jsonify, request

app = Flask(__name__)

# Util objecs.
response_util= responses.responses()

# Response bots.
eliza_bot = eliza.eliza()
learn_bot = chatterbot_facade.chatterbot_facade()
dynamic_bot = webquery.webquery()
emotion_bot = emotion.emotion()

@app.route("/", methods=['GET', 'POST'])
def root(): 
	return jsonify({"response" : "Hello Word!"})

@app.route('/listen', methods=['POST'])
def listen():
	try:
		print("------- new message --------")
		json_response = json.loads(request.get_data().decode('utf-8-sig'))
		print("Received: " + str(json_response))
		query = json_response['content']
		pitch = float(json_response['pitch'])
	except Exception:
		print("Bad request!");
		return jsonify({"response" : "Bad request!"}), 400

	# Default response.
	response = "I could not understand!"
	correct_response = False

	# Smart response.
	# This is in progress...
	try:
		# LearnBot response.
		response = learn_bot.respond(query)
		#response = ""
		if response == "" or response == []:
			raise Exception
		print("LearnBot Responding.")
	except Exception:
		try:
			# Dynamic response
			response = response_util.format_response(dynamic_bot.respond(query))
			if response == "" or response == []:
				raise Exception
			print("DynamicBot Responding.")
			# Train LearnBot with the response
			learn_bot.train(query, response)
		except:
			# Fall-back response
			response = eliza_bot.respond(query)
			print("Eliza Responding.")

	# Detect emotion.
	emotion_paramaters = emotion_bot.predict(query,pitch)

	response = {
		"response"   : response_util.format_response(response),
		"emotion"    : emotion_paramaters["emotion"],
		"confidence" : emotion_paramaters["confidence"]
	}
	print("Final Response: " + str(response))
	return jsonify(response)  

if __name__ == '__main__':
	handler = RotatingFileHandler('server.log', maxBytes=10000, backupCount=1)
	handler.setLevel(logging.INFO)
	app.logger.addHandler(handler)
	app.run(host="127.0.0.1",port=5000)

