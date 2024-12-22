# Github Repository Analyzer

* A web scraping project where user can search can search repositories on their network (whom they follow) based on text and language. 
* Implemented proper authentication mechanism (autheticate with Access token and Refresh token)

****Frontend Tools : ReactJs, TypeScript, TailwindCSS.**** <br />
****Backend Tools : .NET Core, PostgreSQL, Entity Framework Core, Redis, RabbitMQ, Docker.****

# Features

### Search Repository
Some we want to know or find some related repositories some specific topic or language. So we can search it on a user repository list. But it is very time consuming as we have to search on every individual user repository List. So in this website you can bulk search on your network whom you follow based on specific text or languages which will save your lots of time.
![Search Repository Feature](https://github.com/Hassan-Jahid17/Github-Repository-Analyzer/blob/master/images/search-features1.jpg)

Desing Dcument and Architechture:
When API received a request from the client, it made an entry on a queue to a message broker which is `SearchRepositoryOnUserNetwork`, and immediately return a response containing the unique `GUID` of the request. After that the client continuously check if the response is ready by doing polling.
After that the request is being process by SearchRepositoryOnUserNetwork Consumer. this consumer crawl the github handle of all the following user of the search user. After that this consumer made entry of each 10 users to the another consumer which is called SearchRepositories Consumer. This SearchRepositories Consumer is actually responsible to crawl the actual repository of a user for the search langauage, text and so on.
When this consumer completed the search of the repository, it save this result to the `redis` so that later we can search this result from the api.
