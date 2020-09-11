ErrorGeneratingOutputMarshaling Session or Host object to secondary AppDomain failed. Check that Host and all types in Session dictionary are Serializable or derive from MarshalByReferenceObject.
An Exception was thrown while running the transformation code. The process cannot continue.  The following Exception was thrown:
System.Runtime.Serialization.SerializationException: The constructor to deserialize an object of type 'VSIXProject.TemplateUtils.TTSettingsDictionary' was not found.
   at Microsoft.VisualStudio.TextTemplating.TransformationRunner.PrepareTransformation(TemplateProcessingSession session, String source, ITextTemplatingEngineHost host)
   at Microsoft.VisualStudio.TextTemplating.Engine.CompileAndPrepareRun(String generatorCode, ITextTemplatingEngineHost host, TemplateProcessingSession session, IDebugTransformationRunFactory runFactory, ITelemetryScope scope)
--- End of stack trace from previous location where exception was thrown ---
   at Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.WatsonReport.GetClrWatsonExceptionInfo(Exception exceptionObject)
