# UsersGitHubRepositories

This App gets users Repositories List With GitHub API 

It Uses gitHub api to request :

	1. Connects to GitHub API and gets all the users that match a search parameter \(string\). 

	   For example, looking with the string \"Colt\" should result in a list of users: 
		
	  \[\"Colt\", \"coltonoscopy\", \"colthreepv\", \"ColtonPhillips\"\]
		
	2. For each user, connects to the GitHub API to get a list of his/hers repositories. 
		
	   For example, Colt's repositories are 

	  \[\"323-backbone-on-rails-part-1\", \"addressbook\", \"ajax\_api\_demo\"\]
	 
	3. Store the user data in the \_MG\_Users table. 
		
	   Store the repository data in the \_MG\_Repositories table.
    
 Use SQL Server and T-Sql Commands .
 
 Use Windows Form to display the results.
 
 Use GitHub Api.
 
__________________________________________________________________________________________________________________ 
 * **GitHub Api restrict for only 1000 Search Results per Request.**
 * **GitHub Api restrict for only 60 requests per hour without Authorization.**
 * **GitHub Api Limit Requests to 5000 per hour with Basic Authentication.**
 * **No Use Of Background Worker to make UI responding.**
 * **N/A Is Repository without Language**
___________________________________________________________________________________________________________________ 

 **SQL Queries**

 **_I_** 

	USE GitHubRepositories

	SELECT count\(UserId\) As Number,'0-50' As Category

	FROM \[dbo\].\[\_MG\_Users\]

	WHERE followers <= 50 

	UNION

	SELECT count\(UserId\) As Number,'51-500' As Category

	FROM \[dbo\].\[\_MG\_Users\]

	WHERE followers between 51 and 500

	UNION

	SELECT count\(UserId\) As Number,'500+' As Category

	FROM \[dbo\].\[\_MG\_Users\]

	WHERE followers > 500 


	**_II_**

	SELECT B.Language,Count\(B.UserId\) AS Number

	FROM 

		\(SELECT C.UserID,MAX\(C.UserNumberOfRepo\) AS UserTopLanguage
		
		FROM \(SELECT UserID,Language,count\(Language\) AS UserNumberOfRepo
		
				FROM \_MG\_Repositories
				
				GROUP BY UserID,Language\) AS C
				
		GROUP BY UserID\) AS A
		
	LEFT OUTER JOIN 

		\(SELECT UserID,Language,count\(Language\) AS UserNumberOfRepo
		
		FROM \_MG\_Repositories
		
		GROUP BY UserID,Language\) AS B
		
	ON A.UserID = B.UserID And A.UserTopLanguage = B.UserNumberOfRepo

	GROUP BY B.Language
