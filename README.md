# Github Repository Analyzer

* A web scraping project where user can search can search repositories on their network (whom they follow) based on text and language. 
* Implemented proper authentication mechanism (autheticate with Access token and Refresh token)

****Frontend Tools : ReactJs, TypeScript, TailwindCSS.**** <br />
****Backend Tools : .NET Core, PostgreSQL, Entity Framework Core, Redis, RabbitMQ, Docker.****

# Features

### Search Repository
Some we want to know or find some related repositories some specific topic or language. So we can search it on a user repository list. But it is very time consuming as we have to search on every individual user repository List. So in this website you can bulk search on your network whom you follow based on specific text or languages which will save your lots of time.
![Search Repository Feature](https://github.com/Hassan-Jahid17/Github-Repository-Analyzer/blob/master/images/search-features1.jpg)

## **Design document and architecture**

<img width="550" src="https://github.com/user-attachments/assets/3e3d2f7c-9b36-443d-993e-2c2dffd6d3a8" />



1. **API Layer**
   - Receives requests from the client.
   - Generates a unique `GUID` for each request.
   - Returns the `GUID` to the client.
   - Clients use a polling mechanism to check if the request is ready.
   - Enqueues the request to the `queue:SearchRepositoryOnUserNetwork` for processing.

2. **Message Broker**
   - Facilitates communication between different services using queues:
     - **queue:SearchRepositoryOnUserNetwork**: Processes user network crawling tasks.
     - **queue:SearchRepositoryOnUserNetwork**: Handles repository search tasks.

3. **Consumers**
   - **SearchRepositoryOnUserNetworkConsumer**:
     - Consumes tasks from `queue:SearchRepositoryOnUserNetwork`.
     - Check if the desire result exist in the `Redis`, if not crawls the GitHub network of the specified user (e.g. following users) and save to redis.
     - Splits tasks into batches of 10 users and enqueues them into `RepositorySearchQueue`.
   - **SearchRepositoriesConsumer**:
     - Consumes batches of users from `RepositorySearchQueue`.
     - Crawls repositories for each user based on the search criteria (e.g., language, text).
     - Stores the results in a cache.

4. **Storage**
   - **Primary Database (PostgreSQL):**
     - Stores user login information.
   - **Cache (Redis):**
     - Caches frequently accessed results for quick retrieval. In this system cache has been used for storing user networking information (e.g. following users)

5. **Client Polling Mechanism**
   - Clients periodically poll the API using the `GUID` to check the status of their request.

---

## **Workflow**

### Step 1: Client Request
- The client sends a search request to the API.
- The API generates a unique `GUID`, and returns the `GUID` to the client.
- The API enqueues the request into the `queue:SearchRepositoryOnUserNetwork`.

### Step 2: User Network Crawling
- **SearchRepositoryOnUserNetworkConsumer** consumes tasks from `queue:SearchRepositoryOnUserNetwork`.
- It crawls the GitHub user network (followers or following) of the specified user.
- For every 10 users, it creates a batch and enqueues it into `queue:SearchRepositories`.

### Step 3: Repository Crawling
- **SearchRepositoriesConsumer** consumes tasks from `queue:SearchRepositories`.
- It crawls the repositories of each user in the batch for the specified search criteria (e.g., language, text).
- Stores the results in the `Redis Cache`.

### Step 4: Client Polling
- The client periodically polls the API using the `GUID` to check if the request is `completed`.

---

### Api Documentation

Initiate a Repositories Search : `POST api/search/repositories`

```json
Request Body
{
    "GithubHandle" : "jahidhasan17",
    "QueryText" : "Algorithm",
    "QueryLanguage" : "c++",
    "SearchStartIndexFromFollowingUsers": 0
}
```

```json
Response Body
{
    "message": "Repositories retrieved successfully.",
    "data": {
        "searchId": "412d4387-2e5c-416b-84a5-2c8b1827f97b"
    }
}
```
GET search Repositories : `GET api/get/repositories?searchId=id`

```json
Response Body
{
    "message": "Search completed.",
    "data": [
        {
            "name": "Data-Structures-and-Algorithms-Notebook",
            "userName": "michaelstaib",
            "forkName": null,
            "description": "Open Source DSA notebooks",
            "language": "Python",
            "star": "52",
            "fork": "72",
            "modifiedDate": "Mar 26, 2023"
        },
    ]
}
```

#### Identity Apis

Sign up : `POST api/Identity/signup`

```json
Request Body
{
    "UserName" : "Jahid",
    "Email" : "jahid@gmail.com",
    "Password" : "123456"
}
```

```json
Response Body
{
    "message": "Account created successfully"
}
```

Login : `POST api/Identity/login`
```json
Request Body
{
    "Email" : "jahid@gmail.com",
    "Password" : "123456"
}
```

```json
Response Body
{
    "accessToken": "eyJhbGciOiJ.eyJodHRwOi8vc2NoZW1hcy54bWxzb2F.nLOn0fJvzrWm",
    "refreshToken": "5FiT9cvG4OyU2oyLVI57flcC2qBijzs11xGo+GECAA5UmEHQQtCCOnUy6xzh0MKGPqanh9bQWkc5rfA6Kntjgw==",
    "message": "Logged in successfully"
}
```
