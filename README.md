Author:     Adam Isaac
Partner:    None
Start Date: 1-12-2024
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  adamisaac2
Repo:       https://github.com/adamisaac2/Speadsheet
Commit Date: 1-18-2024
Solution:   Spreadsheet
Copyright:  CS 3500 and [Adam Isaac] - This work may not be copied for use in Academic Coursework.

# Overview of the Spreadsheet functionality, Updated After Assignment 6


The Spreadsheet program is currently capable of not very much. I definitely underestimated
how easy this was going to be. Its extremely buggy at the moment and can only do a certain amount
of features. It is especially buggy when it contains complex expressions using parenthesis and when 
variables and parenthesis are used in the same expression.Future extensions are bug fixing and 
getting variables to correctly work with parenthesis. -- Update A3 -- While the spreadsheet functionality still is not available, I have
updated the mathematical side of it, so that the spreadsheet(when implemented) can actually evaluate full functions using PEMDAS without
completely being wrong. It can now evaluate expressions correctly. --Update A4 -- Spreadsheet can now use cell functionality and can use
information from other cells in the spreadsheet. While the GUI is not set up yet, everything seems to be getting better and better in terms
of functionality. --Update A5 -- Spreadsheet now uses XML docs and can save data to xml doc. Also changed a few protection levels on methods 
that were previously public, now set to protected. --Update A6-- Spreadsheet now has a partially complete spreadsheet GUI. It works kind of
but doesnt have any special features and is still really buggy. I have most of the requirements but I am done stressing myself out over this.
I wish I had worked witha  partner but it is what it is. Im taking the L so i can study for the midterm next week. I have conceded defeat. 
I will see you next semester professor. 

# Time Expenditures:

    1. Formula Evaluator:   Predicted Hours:          10        Actual Hours:   20
    2. Formula Evaluator Tester:   Predicted Hours:           4        Actual Hours:    5

    1. DependencyGraph: Predicted Hours: 10   Actual Hours: 10
    2. DependencyGraphTester: Predicted Hours: 5 Actual Hours: 2

    1. Formula: Predicted Hours: 10 Actual hours: ~15
    2. FormulaTests: Predicted Hours: 3 Actual Hours: 3

    1. Spreadsheet: Predicted Hours: 10 Actual Hours : 10
    2. SpreadsheetTests: Predicted Hours: 3 Actual Hours: 3

    1. Spreadsheet A5: Predicted Hours: 10 Actual Hours: 11
    2. SpreadsheetTests A5: Predicted Hours: 2 Actual Hours : 3.5

    1. Spreadsheet GUI A6: Predicted Hours: 25+ Actual Hours: ~20
    

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

    Time Breakdown Assingment 5 - 
    I really spent a good amount of time revising the old methods that got changed, idk why it was so hard for me but I felt like I struggled 
    a little bit lol. After that I had a pretty good time on implementing the new methods that contained the XML stuff. I had a feeling it wasnt
    gonna take me THAT long, and I was right I think it took me around like 10 hours including bug fixing which I personally dont think is that bad
    for me. And I think ima pretty shit programmer lol. Good thing I like doing it lol. My coverage is 95.5 percent which is weird because my last
    test that I made, I checked before running it and I was at 95.5 percent, then ran it and it was still at 95.5 percent except the lines that were
    red before, were green which means my test covered the previously red lines and my test coverage didnt go up, which means other lines that were green
    before, turned red. So idfk whats going on ig. 

    Time Breakdown Assignment 6 -
    I spent alot of time on this assignment. I didnt work with a partner and it definitely came back to bite me in the ass.
    The first week we got assigned this assignment I could barely work on it because I had an exam in another class that same week
    and spent most of my time studying for it. I did do well on the exam but in the end I didnt give myself enough time to work on this
    assignment and I tried to fit it all in to a few days and it didnt work. The GUI is like partially complete but im done stressing myself
    over it and just taking the L so I can study for the exam. I would say that my time spent on the GUI was spent pretty evenly amongst
    the implementations of the GUI, it took me a pretty much equal amount of time to implement everything I have available in the GUI and I probably
    worked like 20 or so hours on it. 