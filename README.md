Author:     Adam Isaac
Partner:    None
Start Date: 1-12-2024
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  adamisaac2
Repo:       https://github.com/adamisaac2/Speadsheet
Commit Date: 1-18-2024
Solution:   Spreadsheet
Copyright:  CS 3500 and [Adam Isaac] - This work may not be copied for use in Academic Coursework.

# Overview of the Spreadsheet functionality, Updated After Assignment 3


The Spreadsheet program is currently capable of not very much. I definitely underestimated
how easy this was going to be. Its extremely buggy at the moment and can only do a certain amount
of features. It is especially buggy when it contains complex expressions using parenthesis and when 
variables and parenthesis are used in the same expression.Future extensions are bug fixing and 
getting variables to correctly work with parenthesis. -- Update A3 -- While the spreadsheet functionality still is not available, I have
updated the mathematical side of it, so that the spreadsheet(when implemented) can actually evaluate full functions using PEMDAS without
completely being wrong. It can now evaluate expressions correctly. --Update A4 -- Spreadsheet can now use cell functionality and can use
information from other cells in the spreadsheet. While the GUI is not set up yet, everything seems to be getting better and better in terms
of functionality. 

# Time Expenditures:

    1. Formula Evaluator:   Predicted Hours:          10        Actual Hours:   20
    2. Formula Evaluator Tester:   Predicted Hours:           4        Actual Hours:    5

    1. DependencyGraph: Predicted Hours: 10   Actual Hours: 10
    2. DependencyGraphTester: Predicted Hours: 5 Actual Hours: 2

    1. Formula: Predicted Hours: 10 Actual hours: ~15
    2. FormulaTests: Predicted Hours: 3 Actual Hours: 3

    1. Spreadsheet: Predicted Hours: 10 Actual Hours : 10
    2. SpreadsheetTests: Predicted Hours: 3 Actual Hours: 3

    Time Breakdown Assignment 1 - 
    My first day working on this, I spent about 4 or 5 hours just writing all the base code without debugging
    to kind of work my way and get a feel for how I thought the code would best work. The next day I spent about 4
    hours getting the tests set up and debugging the testing software which was kind of a pain. I then spent
    about 5 more hours finishing up the code and started debugging. I thought my base code was gonna be pretty good
    but I was completely wrong. It was a complete buggy mess that didnt really work half the time so I spent about 
    10 hours through 2 more days debugging and writing new code, and fixing that code. In total I spent about 
    20 hours on the source code and probably 5 on the tester.

    Time Breakdown Assignment 2 - 
    Not alot to say about the time spent on this assignment. I probably spent around 10 hours total, most of it on writing 
    the code and then probably 2 hours debugging and getting the tests to work. I would say that the tests provided to us 
    were adequate at testing the big cases while I only needed to really look into a small number of problems regarding other methods. 
    I then made a few extra test cases in the Tester class to ensure my code coverage was as much as I could possibly get. 

    Time Breakdown Assignment 3 -
    I didnt think I would be spending alot of time on the source code because I had already done it in assignment 1 and then revised
    it, but of course, I had to spend more time then I had anticipated because I kept running into walls. Mostly because when I first
    implemented the formula method, I had messed up on the way the strings were added to the tokens list and I couldnt figure out what
    was going wrong for the longest time, and come to find out it was literally only like 2 lines of code I was missing that was messing
    everything else up lol. After I fixed that it was pretty much good except for writing the tests took alot of time as well. 

    Time Breakdown Assignment 4 -
    I feel like I had a fairly straight forward time with this assignment. My implementations of methods went smoothly and I didnt really 
    have that many bug encounters or code that wasnt really working, except for one instance where I had misplaced the position of variables 
    in my SetCellContents for the string and double. Which caused a bug that took me a while to figure out because I had thought it was in 
    a different method. Besides that I would say that everything went well. My coverage is as 97 percent and I had made more then a few tests 
    that cover edge cases, so I think I will be good for grading on this assingmnet. 
