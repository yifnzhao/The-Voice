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
from models.eliza import eliza
from models.chatterbot import chatterbot_facade
from stubs.emotion import emotion

from logging.handlers import RotatingFileHandler

from flask import Flask, jsonify, request

app = Flask(__name__)

# Response bots.
eliza_bot = eliza.eliza()
chatter_bot = chatterbot_facade.chatterbot_facade()
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
		print("Some Exception");
		return jsonify({"response" : "Bad request!"}), 400

	# Default response.
	response = "I could not understand!"

	# Smart response.
	# This is in progress...
	try:
		# Chatterbot response.
		# This will time out and throw exception
		# if an answer is not generated.
		response = chatter_bot.respond(query)
		print("ChatterBot Responding.")
	except Exception:
		# Fall-back response
		response = eliza_bot.respond(query)
		print("Eliza Responding.")

	# Detect emotion.
	emotion_paramaters = emotion_bot.predict(query,pitch)

	# Shorten the answer limit to 256 characters if longer.
	if len(response) > 256: 
		# Get first 256 characters.
		# We know just doing this might result with incomplate responses but it is ok for now.
		response = response[:256]

	response = {
		"response"   : response,
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

