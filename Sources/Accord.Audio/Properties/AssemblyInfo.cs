#region Using directives

using System;
using System.Reflection;
using System.Runtime.InteropServices;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Accord.Audio")]
[assembly: AssemblyDescription("Accord.NET - Audio Library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Accord.NET")]
[assembly: AssemblyProduct("Accord.Audio")]
[assembly: AssemblyCopyright("Copyright © César Souza 2009-2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and 
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion("2.3.0.0")]
[assembly: AssemblyFileVersionAttribute("2.3.0.0")]


[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Accord.Audio.PlayFrameEventArgs.#Empty")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "Accord.Audio.Signal.#GetEnergy()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "param", Scope = "member", Target = "Accord.Audio.UnsupportedSampleFormatException.#.ctor(System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Accord.Audio.Extensions.#RawDeserialize`1(System.Byte[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Accord.Audio.Extensions.#RawDeserialize`1(System.Byte[],System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "sampleRate", Scope = "member", Target = "Accord.Audio.Signal.#init(System.Byte[],System.Int32,System.Int32,System.Int32,Accord.Audio.SampleFormat)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "length", Scope = "member", Target = "Accord.Audio.Signal.#init(System.Byte[],System.Int32,System.Int32,System.Int32,Accord.Audio.SampleFormat)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "format", Scope = "member", Target = "Accord.Audio.Signal.#init(System.Byte[],System.Int32,System.Int32,System.Int32,Accord.Audio.SampleFormat)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "channels", Scope = "member", Target = "Accord.Audio.Signal.#init(System.Byte[],System.Int32,System.Int32,System.Int32,Accord.Audio.SampleFormat)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "channels", Scope = "member", Target = "Accord.Audio.ComplexSignal.#Create(System.Int32,System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Scope = "member", Target = "Accord.Audio.IAudioOutput.#Stop()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Scope = "member", Target = "Accord.Audio.IAudioSource.#Stop()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "from-128", Scope = "member", Target = "Accord.Audio.SampleConverter.#Convert(System.Byte,System.Int16&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "from-128", Scope = "member", Target = "Accord.Audio.SampleConverter.#Convert(System.Byte,System.Int32&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "from-128", Scope = "member", Target = "Accord.Audio.SampleConverter.#Convert(System.Byte,System.Single&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Accord.Audio.Generators")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Accord.Audio.AudioSourceErrorEventArgs.#Empty")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "param", Scope = "member", Target = "Accord.Audio.InvalidSignalPropertiesException.#.ctor(System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "length+1", Scope = "member", Target = "Accord.Audio.Tools.#GetFrequencyVector(System.Int32,System.Int32)")]

