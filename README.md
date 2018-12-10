# UsersGitHubRepositories
Thie App gets users Repositories List With GitHub API

The UsersGitHubRepositories app use gitHub api to request :
1.	Connects to GitHub API and gets all the users that match a search parameter (string). 
    For example, looking with the string "Colt" should result in a list of users: ["Colt", "coltonoscopy", "colthreepv", "ColtonPhillips" …. ]
    
2.	For each user, connects to the GitHub API to get a list of his/hers repositories. 
    For example, Colt's repositories are ["323-backbone-on-rails-part-1", "addressbook", "ajax_api_demo" …]
 
3.	Store the user data in the _MG_Users table. 
    Store the repository data in the _MG_Repositories table.
    
 Use SQL Server and T-Sql Commands .
 Use Windows Form to display the results.
 Use GitHub Api.
 
 * GitHub Api restrict for only 1000 Search Results per Request.
 * GitHub Api restrict for only 60 requests per hour. 
 
 ===============
 SQL Queries
 ===============
 
 
