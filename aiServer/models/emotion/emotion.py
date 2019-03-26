#----------------------------------------------------------------------
#  emotion.py
#
# The Voice : Predict emotion from a text input by word match.
# Author :  Korhan Akcura
#----------------------------------------------------------------------
import re
from random import randint
from thesaurus import Word

class emotion:
	def __init__(self):
		try:
			# Dynamically get from Thesaurus.
			self.emotions= ["happy", "sad"]
			emotion_properties = [Word(self.emotions[0]),Word(self.emotions[1])]
			self.happy_synonyms = list(itertools.chain.from_iterable(emotion_properties[0].synonyms('all')))
			self.sad_synonyms = list(itertools.chain.from_iterable(emotion_properties[1].synonyms('all')))
		except:
			# Use hardcoded values.
			self.happy_synonym = ['happy','cheerful','contented','delighted','ecstatic','elated','glad','joyful','joyous','jubilant','lively','merry','overjoyed','peaceful','pleasant','pleased','thrilled','upbeat','blessed','blest','blissful','blithe','no complain','captivated','chipper','chirpy','content','convivial','exultant','flying high','gleeful','gratified','intoxicated','jolly','laughing','light','looking good','mirthful','on cloud nine','peppy','perky','playful','sparkling','sunny','tickled','tickled pink','up','walking on air']
			self.sad_synonym = ['sad','bitter', 'dismal', 'heartbroken', 'melancholy', 'mournful', 'pessimistic', 'somber', 'sorrowful', 'sorry', 'wistful', 'bereaved', 'blue', 'cheerless', 'dejected', 'despairing', 'despondent', 'disconsolate', 'distressed', 'doleful', 'down', 'down in dumps', 'down in mouth', 'downcast', 'forlorn', 'gloomy', 'glum', 'grief-stricken', 'grieved', 'heartsick', 'heavyhearted', 'hurting', 'in doldrums', 'in grief', 'in the dumps', 'languishing', 'low', 'low-spirited', 'lugubrious', 'morbid', 'morose', 'out of sorts', 'pensive', 'sick at heart', 'troubled', 'weeping', 'woebegone']

	#----------------------------------------------------------------------
	#  Predict the emotion of a text as happy, sad or natural.
	#----------------------------------------------------------------------
	def predict(self,str,pitch):
		# Default prediction.
		prediction = {"emotion": "natural", "confidence": 100}

		# Replace any punctuation with space.
		str = re.sub(r'[^\w\s]',' ',str)

		# Lowercase and break string into individual words
		potential_emotion_words = str.lower().split()

		# Detect if Happy
		if any(s in self.happy_synonym for s in potential_emotion_words):
			prediction["emotion"] = "happy"
		# Detect if Sad
		elif any(s in self.sad_synonym for s in potential_emotion_words):
			prediction["emotion"] = "sad"

		prediction["confidence"] = pitch * 100

		return prediction
