# Exercise 2: Observing Durable Execution
During this exercise, you will

* Create Workflow and Activity loggers 
* Add logging statements to the code
* Add a Timer to the Workflow Definition
* Launch two Workers and run the Workflow
* Kill one of the Workers during Workflow Execution and observe that the remaining Worker completes the execution

Make your changes to the code in the `practice` subdirectory (look for `TODO` comments that will guide you to where you should make changes to the code). If you need a hint or want to verify your changes, look at the complete version in the `solution` subdirectory.

## Prerequisite: Ensure that the Microservice Is Running

**Note: If you're using the Gitpod environment to run this exercise you can
skip this step. An instance of the microservice is already running in your
environment**

If you haven't already started the microservice in previous exercises, do so in
a separate terminal. From either the `practice` or `solution` subdirectory for
this exercise, run `dotnet run --project Web`. The
microservice code does not change between the practice and solution examples.

## Part A: Add Logging to the Workflow Code

1. Edit the `TranslationWorkflow.cs` file in the Workflow directory.
2. Define a Workflow logger at the top of the Workflow function
3. Add a new line after that to log a message at the Info level
   1. It should mention that the Workflow function has been invoked
   2. It should also include the name passed as input
4. Before each call to Execute Activity, log a message at Debug level
   1. It should also include the language code passed as input
5. Save your changes

## Part B: Add Logging to the Activity Code

1. Edit the `Activities.cs` file in the Workflow directory.
2. Define an Activity logger at the top of the Activity function
3. Insert a logging statement at the Info level just after this, so you'll know when the Activity is invoked.
   1. Include the term being translated and the language code
4. Optionally, add log statements at the Error level anywhere that the Activity throws an Exception
5. Near the bottom of the function, use the Debug level to log the successful translation
   1. Include the translated term
6. Save your changes

## Part C: Add a Timer to the Workflow
You will now add a Timer between the two Activity calls in the Workflow Definition, which will make it easier to observe durable execution in the next section.

1. After the statement where `helloMessage` is defined, but before the statement where `byeMessage`is defined, add a new statement that logs the message "Sleeping between translation calls" at the Debug level.
2. Just after the new log statement, use `Workflow.DelayAsync` to set a Timer for 10 seconds

## Part D: Observe Durable Execution
It is typical to run Temporal applications using two or more Worker processes. Not only do additional Workers allow the application to scale, it also increases availability since another Worker can take over if a Worker crashes during Workflow Execution. You'll see this for yourself now and will learn more about how Temporal achieves this as you continue through the course.

Before proceeding, make sure that there are no Workers running for this or any previous exercise. Also, please read through all of these instructions before you begin, so that you'll know when and how to react.

1. In another terminal, start the Worker by running `dotnet run --project Worker`
2. In another terminal, start a second Worker by running `dotnet run --project Worker`
3. In another terminal, start the microserving by running `dotnet run --project Web`
4. In another terminal, execute the Workflow by running `dotnet run --project Client Tatiana sk` (replace `Tatiana` with your first name) 
5. Observe the Event History in the Web UI. 
6. As soon as you see a log message in one of the Worker terminals indicating that it has started the Timer, press Ctrl-C in that window to kill that Worker process.
7. Switch to the terminal window for the other Worker process. Within a few seconds, you should observe new output, indicating that it has resumed execution of the Workflow.
8. Once you see log output indicating that translation was successful, switch back to the terminal window where you started the Workflow. 

After the final step, you should see the translated Hello and Goodbye messages, which confirms that Workflow Execution completed successfully despite the original Worker being killed.

Since you added logging code to the Workflow and Activity code, take a moment to look at what you see in the terminal windows for each Worker and think about what took place. You may also find it helpful to look at this Workflow Execution in the Web UI.

The microservice for this exercise logs each successful translation, and if you look at its terminal window, you will see that the service only translated Hello (the first Activity) once, even though the Worker was killed after this translation took place. In other words, Temporal did not re-execute the completed Activity when it restored the state of the Workflow Execution. 

### This is the end of the exercise.