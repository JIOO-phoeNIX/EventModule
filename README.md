# EventModule
This is a demo project on basic event management like creating an event, participating in an event, sending invitations to participants...

Tech stack used in this POC:
C#,
.NET Core API 6,
EF Core,
SQLite,
Docker

Others:
In-memory cache,
Unit test (XUnit),
Moq,
Dependency injection,
Microservices



## Set up
After cloning the repo, open the root folder that contains the Dockerfile in your terminal. 
Run the below commands one after the other to build the docker image and run the container (ensure Docker desktop is running):

```
docker build -t eventmoduleapi .

docker run --name eventmoduleapi -p 8080:80 -d eventmoduleap
```

You should get the outputs below:

![Docker image build and container run](https://github.com/JIOO-phoeNIX/EventModule/blob/master/TestImages/dockercommands.png)


In Docker desktop, you will see the image as shown below:

![docker image](https://github.com/JIOO-phoeNIX/EventModule/blob/master/TestImages/dockerimagebuild.png)


You can access the application on port [8080](http://localhost:8080/swagger/index.html) as shown below:

![running application](https://github.com/JIOO-phoeNIX/EventModule/blob/master/TestImages/applicationrunningonport.png)



## Endpoints

NOTE: authentication is not included in the project.

POST api/Event/add: this is used to CREATE an event.
When running on linux/docker, sample timezone use:

```
(UTC+01:00) Central European Time (Vatican)

(UTC+03:00) East Africa Time (Nairobi)

(UTC+02:00) South Africa Standard Time (Johannesburg)
```

On Windows (if you are running locally from VS or a Windows server):
```
(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna

(UTC+01:00) West Central Africa

(UTC-08:00) Pacific Time (US & Canada)
```

POST api/Event/update is used to update an event details

GET api/Event/alluserevents/{userId} is used to fetch the paged events of a user

GET api/Event/info/{id} is used to get the details of an event including the user info

POST api/Event/participantregistration is used to register a participant for an event

GET api/Event/alleventparticipants/{eventId} to get all the participants of an event

POST api/Event/inviteparticipants this is used by the owner of the event to send invitations to people (the participantsId param collects the comma-separated invitees Ids)

POST api/Event/acceptinivitationtoevent used by the invitee to accept the event invitation

GET api/Event/alleventstoparticipate/{userId} to get a paged response of all the events a user is participating in
