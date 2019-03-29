#----------------------------------------------------------------------
#  user_input.py
#
# The Voice : Get the answer from user.
# Author :  Korhan Akcura
#----------------------------------------------------------------------

class user_input:
	def __init__(self):
		self.waiting_question = False 
		self.waiting_answer = False
		self.waiting_information = False
		self.question = ""
		self.answer = ""

	#----------------------------------------------------------------------
	#  Get answer from user.
	#----------------------------------------------------------------------
	def respond(self,learn_bot,str):
		if str == "I want you to learn this." and not self.waiting_information:
			self.waiting_information = True
			self.waiting_question = True
			print("UserBot Responding.")
			return "Please tell me the statement you would like me to learn."

		if self.waiting_information:
			if self.waiting_question:
				self.waiting_question = False
				self.waiting_answer = True
				self.question = str
				print("UserBot Responding.")
				return "What is a good response to that?"
			elif self.waiting_answer:
				self.waiting_information = False
				self.waiting_question = False
				self.waiting_answer = False
				self.answer = str
				learn_bot.train(self.question, self.answer)
				print("UserBot Responding.")
				return "I learned that: " + self.question + " " + self.answer
			else:
				self.waiting_information = False
				self.waiting_question = False
				self.waiting_answer = False
				return ""

		return ""
