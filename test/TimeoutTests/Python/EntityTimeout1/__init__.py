import logging
import json
import sys
import azure.functions as func
import azure.durable_functions as df

def orchestrator_function(context: df.DurableOrchestrationContext):
    entityId = df.EntityId("SlowEntity", context.instance_id)
    try:
        print("calling slow entity operation... this cannot end well")
        current_value = yield context.call_entity(entityId, "", 180)
        print("what? no exception?")
        return "test failed: expected exception, but no exception thrown"
    except GeneratorExit:
        print("Exiting generator.")
        raise
    except:
        print("Whew!", sys.exc_info()[0], "occurred.")
        return "test passed: exception thrown"

main = df.Orchestrator.create(orchestrator_function)
