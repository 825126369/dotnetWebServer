from distutils.log import debug
from typing import List
from locust import HttpUser, task, between
import random
import json
from uuid import uuid1

def randomUid():
    return random.randint(100000, 100000000)

def get_uuid_str():
    return str(uuid1())

def get_userId():
    return randomUid()

def get_userNick():
    return get_uuid_str()

def get_icon():
    return get_uuid_str()

class MyUser(HttpUser):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.uid = get_userId()
        self.nSelectActivityId = 0
        # self.uid = str(uuid1())
    
    wait_time = between(1, 3)
    
    @task
    def GetRankList(self):
        response = self.client.post("/JiXianTiaoZhan/GetRankList", 
            headers = {"Content-Type": "application/x-www-form-urlencoded","PWD": "123321456654789987963852741"},
            data={"userId":self.uid}
        )
        
    @task
    def GetActivityInfo(self):
        response = self.client.post("/JiXianTiaoZhan/GetActivityInfo", 
            headers = {"Content-Type": "application/x-www-form-urlencoded","PWD": "123321456654789987963852741"},
            data={}
        )

        activityInfo = json.loads(response.content)
        if self.nSelectActivityId == 0:
            arrayActivityIds = list()
            for index in range(len(activityInfo["ActivityInfo"]["mActivityDataList"])):
                nActivityId = activityInfo["ActivityInfo"]["mActivityDataList"][index]["challengeId"]
                arrayActivityIds.append(nActivityId)

            self.nSelectActivityId = random.choice(arrayActivityIds)
            
    @task
    def UploadChengji(self):
        response = self.client.post("/JiXianTiaoZhan/UploadChengji", 
            headers = {"Content-Type": "application/x-www-form-urlencoded","PWD": "123321456654789987963852741"},
            data={
                    "userId":self.uid, 
                    "nActivityId":self.nSelectActivityId,
                    "nTime": random.randint(1, 3600),
                    "nScore":random.randint(1000, 100000000)
                }
        )
