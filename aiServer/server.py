# The Voice : Main server code for NLP and text responses.
# Author :  Korhan Akcura

import json
import requests
import logging
from models import eliza

from logging.handlers import RotatingFileHandler

from flask import Flask, jsonify, request

app = Flask(__name__)

# Fall-back response bot.
eliza_bot = eliza.eliza()

@app.route("/", methods=['GET', 'POST'])
def root(): 
	return jsonify({"response" : "Hello Word!"})

@app.route('/listen', methods=['POST'])
def listen():
	try:
		query = json.loads(request.data)['content']
	except Exception:
		return jsonify({"response" : "Bad request!"}), 400

	# Default response.
	response = "I could not understand!"

	# Smart response.

	# Fall-back response.
	response = eliza_bot.respond(query)

	response = {
		"response" : response
	}
	return jsonify(response)  

if __name__ == '__main__':
	handler = RotatingFileHandler('server.log', maxBytes=10000, backupCount=1)
	handler.setLevel(logging.INFO)
	app.logger.addHandler(handler)
	app.run(host="127.0.0.1",port=5000)

