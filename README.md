# Introduction 
Azure Functions are the core compute component of Microsof's "serverless" platform. Functions are small pieces of code that
execute as a reaction to an event (or trigger).  Typically functions are short-lived and stateless. And for the vast majority
of use cases, these are not really limitations, but in every real world application at some point you will need to do some
background processing or execute some sort of workflow.  Can you still use functions for these types of operations? And can 
a long running workflow maintain state and update interested parties with their state as they progress through the workflow?

In this session we are going to try and answer those questions by working through a series of small demos.  Here's what I hope 
to cover over the next 40-50 minutes.

1. Dependency injection with functions
1. Creating new durable functions and querying existing functions to get their status
1. Serving up static content, so that we can show a simple UI to consume our functions
1. Using SignalR to report back status in real-time

---

**David Judd - Managing Pricipal and App Dev Practice Lead**

**ObjectSharp**

devsummit@objectsharp.com
