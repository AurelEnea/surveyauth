Survey for authenticated users.
First the user is directed to the login page.
After successful login, the home page displays a drop down list of surveys to choose from.
Selecting a survey activates the button below, to go to answer questions associated with the selected survey.
The user becomes Respondent and he/she can add, edit and save changes to answers.

Version v3 introduced dynamic display of items: the page ItemList.razor displays both Questions and Surveys. 
It receives the type of item as @typeparam and reads its properties to decide what/how to display.
