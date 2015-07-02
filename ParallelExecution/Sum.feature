Feature: Parallel
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Scenario: Add two numbers
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen

Scenario: Add two numbers 2
	Given I have entered 50 into the calculator
	And I have entered 80 into the calculator
	When I press add
	Then the result should be 130 on the screen

@AddValue10BeforeScenario
Scenario: Add two numbers 3
	Given I have entered 50 into the calculator
	And I have entered 80 into the calculator
	When I press add
	Then the result should be 140 on the screen

Scenario: Add two numbers 4
	Given I have entered 1 into the calculator
	And I have entered 2 into the calculator
	When I press add
	Then the result should be 3 on the screen

Scenario: Add two numbers 5
	Given I have entered 4 into the calculator
	And I have entered 5 into the calculator
	When I press add
	Then the result should be 9 on the screen

Scenario: Add two numbers 6
	Given I have entered 8 into the calculator
	And I have entered 4 into the calculator
	When I press add
	Then the result should be 12 on the screen

Scenario: Add two numbers 7
	Given I have entered 9 into the calculator
	And I have entered 10 into the calculator
	When I press add
	Then the result should be 19 on the screen

@AddValue10BeforeScenario
Scenario: Add two numbers 8
	Given I have entered 5 into the calculator
	And I have entered 8 into the calculator
	When I press add
	Then the result should be 23 on the screen

Scenario: Add two numbers 9
	Given I have entered 1 into the calculator
	And I have entered 1 into the calculator
	When I press add
	Then the result should be 3 on the screen

Scenario: Add two numbers 10
	Given I have entered 4 into the calculator
	And I have entered 8 into the calculator
	When I press add
	Then the result should be 12 on the screen

Scenario: Add two numbers 11
	Given I have entered 500 into the calculator
	And I have entered 80 into the calculator
	When I press add
	Then the result should be 580 on the screen

Scenario: Add two numbers 12
	Given I have entered 2 into the calculator
	And I have entered 4 into the calculator
	When I press add
	Then the result should be 6 on the screen