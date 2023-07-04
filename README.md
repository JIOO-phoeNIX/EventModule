# EventModule
This is a demo project on basic event management like creating an event, participating in an event, sending invitations to participants...

Tech stack used in this POC:
C#
.NET Core API 6
EF Core
SQLite
Docker

Others:
In-memory cache
Unit test (XUnit)
Moq
Dependency injection
Microservices


## Set up
After cloning the repo, open the root folder that contains the Dockerfile in your terminal. 
Run the below commands one after the other to build the docker image and run the container (ensure Docker desktop is running):

```
docker build -t eventmoduleapi .

docker run --name eventmoduleapi -p 8080:80 -d eventmoduleapi

```

You should get the outputs below:

![Docker image build and container run](https://github.com/JIOO-phoeNIX/EventModule/blob/master/TestImages/dockercommands.png)


In Docker desktop, you will the image as shown below:

![docker image](https://github.com/JIOO-phoeNIX/EventModule/blob/master/TestImages/dockerimagebuild.png)


You can access the application on port [8080](http://localhost:8080/swagger/index.html) as shown below:

![running application](https://github.com/JIOO-phoeNIX/EventModule/blob/master/TestImages/applicationrunningonport.png)


## Endpoints
