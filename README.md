# UnityLint

UnityLint is a tool for detecting bad game smells in Unity video games. It is able to detect 18 types of video game bad smells defined in a previous research work ([Nardone et al., 2022](https://mdipenta.github.io/files/tosem-gamesmells.pdf)): *Vittoria Nardone, Biruk Asmare Muse, Mouna Abidi, Foutse Khomh, and Massimiliano Di Penta. 2022. Video Game Bad Smells: What they are and how Developers Perceive Them. ACM Trans. Softw. Eng. Methodol. (September 2022). https://doi.org/10.1145/3563214 * 

UnityLint detects the following video game smells belonging to different categories:

**Design and Game Logic**

- Bloated assets
- Creating components/objects at run-time
- Dependencies between objects
- Lack of separation of concerns
- Poor design of object state management
- Static coupling
- Search by string/ID
- Singleton vs. static
- Weak temporization strategy

**Animation**

- Continuously checking position/rotation
- Multiple Animators over model component
- Too many Keyframes in animations
- Use of anystate in animator controller

**Physics**

- Heavy-weight physics computation in game objects updates
- Improper mesh settings for a collider
- Setting object velocity and override forces

**Rendering**

- Lack of optimization when drawing/rendering objects
- Sub-optimal, expensive choice of lights, shadows, or reflections


### Table of Contents

**[Download and Installation](#download-and-installation)**
**[Starter Tool](#starter-tool)**
**[CSharpAnalyzer](#csharpanalyzer)**
**[CodeSmellAnalyzer](#codesmellanalyzer)**
**[UnityDataAnalyzer](#unitydataanalyzer)**
**[MetaSmellAnalyzer](#metasmellanalyzer)**
**[Recompiling UnityLint](#recompiling-unitylint)**

## Download And Installation

You can [download](https://github.com/mdipenta/UnityCodeSmellAnalyzer/releases/) the latest available release of UnityLint.

Alternatively, you can [recompile](#recompiling-unitylint) the tool using Visual Studio.

### Requirements

The tool works natively under Windows 10 or above. Also, the tool works under Linux and Mac OS, and requires you to install [Mono](https://www.mono-project.com/).

## Starter Tool

ShellStarter is the execution file of UnityLint tool. Given a project or a list of projects, it detects video game smells. 

### How to Use it

Open your command line/terminal and navigate to the directory in which downloaded exe files are located.

Type the following command to run **ShellStarter** tool (depending on your OS (Windows or MacOS/Linux))

*Windows*
```bash
ShellStarter.exe -d <dirPath>
```

*MacOS / Linux*
```bash
mono ShellStarter.exe -d <dirPath>
```

where the **Required Argument** is:
```bash
-p, --project       Project directory.
```

Furthermore, you can use the following **Optional Arguments**:
```bash
-v, --verbose      Display Log on the standard output.

--help             Display this help screen.

--version          Display version information.
```

## CSharpAnalyzer

CSharpAnalizer extracts information from the project's source code using API of Roslyn compiler. It provides as output a JSON file containing all information extracted from source code.

### How to Use it

Open your command line/terminal and navigate to the directory in which downloaded exe files are located.

Type the following command to run **CSharpAnalyzer** tool (depending on your OS (Windows or Mac/Linux))

*Windows*
```bash
CSharpAnalyzer.exe -p <projectDirPath>
```

*MacOS / Linux*
```bash
mono CSharpAnalyzer.exe -p <projectDirPath>
```

where the **Required Argument** is:
```bash
-p, --project       Project directory.
```

Furthermore, you can use the following **Optional Arguments**:
```bash
-r, --results       Directory where to store the results (CodeAnalysis.json file). 
                    If not provided, results are saved in the current directory.

-d, --directory     Analyze the specified directory only. 
                    If not provided, the project directory is selected.

-a, --assembly      Additional assemblies directory (i.e., to analyze DLLs).

-s, --statements    Output all statements raw code in the JSON.

-n, --name          The project name.

-c, --config        Configuration File.

-l, --log           (Default: 1) Log Level: Trace 0 Debug 1 Information 
                    2 Warning 3 Error 4 Critical 5 None 6 (Debug is Default).

-v, --verbose       Displays the log on the standard output.

--help              Displays this help screen.

--version           Displays version information.
```

## CodeSmellAnalyzer

CodeSmellAnalyzer applies detection rules and identifies video game smells located into source code. It uses as input JSON file produced by CSharpAnalyzer.

### How to Use it

Open your command line/terminal and navigate to the directory in which downloaded exe files are located.

Type the following command to run **CodeSmellAnalyzer** tool (depending on your OS (Windows or Mac/Linux))

*Windows*
```bash
CodeSmellAnalyzer.exe -d <JSON-filePath>
```
or 
```bash
CodeSmellAnalyzer.exe -e
```

*MacOS / Linux*
```bash
mono CodeSmellAnalyzer.exe -d <JSON-filePath>
```
or 
```bash
mono CodeSmellAnalyzer.exe -e
```

where the **Required Argument** mutually exclusive can be:
```bash
-e, --expose      Required. Exposes all possible smell names, Mutually 
exclusive with -d, --data

-d, --data        Required. json file name produced by the Code Analyzer, 
Mutually exclusive with -e, --expose

```

Furthermore, you can use the following **Optional Arguments**:
```bash
-s, --smell       Searches for a single smell

-f, --file        Textual file with the list of smells to search for (use the
names produced by the -e option)

-v, --verbose     Enables the status log on the console window

-p, --project     Saves the number of smells for the project in a .csv file

-c, --category    Saves the smells by category

-r, --result      Saves results into a specified folder

-l, --log         Log Level: Trace 0 Debug 1 Information 2 Warning 3 Error 4 
Critical 5 None 6 (Debug is Default)

--help            Displays this help screen.
```

## UnityDataAnalyzer

UnityDataAnalyzer extracts information from the project's metadata (i.e., from files with the following extensions: .unity, .controller, .prefab, .mat, .anim, .flare, .assets and .meta). It provides as output a JSON file containing all information extracted from source code.

### How to Use it

Open your command line/terminal and navigate to the directory in which downloaded exe files are located.

Type the following command to run **UnityDataAnalyzer** tool (depending on your OS (Windows or Mac/Linux))

*Windows*
```bash
UnityDataAnalyzer.exe -a <assetDirPath> 
```

*Mac / Linux*
```bash
mono UnityDataAnalyzer.exe -d <assetDirPath>
```

where the **Required Argument** is:
```bash
-d, --dir        Required. Path to the Assets directory
```

Furthermore, you can use the following **Optional Arguments**:
```bash
-m, --nometa     If specified, the tool does no load .meta files

-f, --fileExt    File (default Extension.txt) containing the extensions to analyze

-e, --ext        List of extensions to search

-v, --verbose    Enable the status log on the console window

-r, --results    Saves results to specified folder (default is the current directory)

-n, --name       Specify the project Name

-l, --log        Log Level: Trace 0 Debug 1 Information 2 Warning 3 Error 4 Critical 5 None 6 (Debug is Default)

--help           Displays this help screen.

--version        Displays version information.
```

## MetaSmellAnalyzer

MetaSmellAnalyzer applies detection rules and identifies video game smells located into metadata. It uses as input JSON file produced by UnityDataAnalyzer.

### How to Use it

Open your command line/terminal and navigate to the directory in which downloaded exe files are located.

Type the following command to run **MetaSmellAnalyzer** tool (depending on your OS (Windows or Mac/Linux))

*Windows*
```bash
MetaSmellAnalyzer.exe -d <JSON-filePath>
```
or 
```bash
MetaSmellAnalyzer.exe -e
```

*MacOS / Linux*
```bash
mono MetaSmellAnalyzer.exe -d <JSON-filePath>
```
or 
```bash
mono MetaSmellAnalyzer.exe -e
```

where the **Required Argument** mutually exclusive can be:
```bash
-e, --expose      Required. Lists all possible smell names (saving them in 
the smellsmethods.txt file). Mutually exclusive with -d, 
--data

-d, --data        Required. Main data directory and Metadata directory path, 
Mutually exclusive with -e, --expose
```

Furthermore, you can use the following **Optional Arguments**:
```bash
-f, --file        Textual file with the list of smells to search (use the 
names produced by the -e option)

-v, --verbose     Enables the status log on console window

-r, --results     Saves results to the specified directory

-c, --category    Saves results by smell category

-p, --project     Saves results as .csv file for the project

-l, --log         Log Level: Trace 0 Debug 1 Information 2 Warning 3 Error 4 
Critical 5 None 6 (Debug is Default)

--help            Display this help screen.

--version         Display version information.
```

> **Note**
> MetaSmellAnalyzer detects some smells using thresholds and/or fixed values. These values are defined in the [smell.txt](https://github.com/mdipenta/UnityCodeSmellAnalyzer/blob/main/Analyzer/MetaSmellAnalyzer/smell.txt) file.
> Values used are explained in the following:
> - m_CollisionDetection: this variable is used to detect Heavy Phisics Computation for collision between Rigidbody collider and other colliders in the scene. The smell exists if this m_CollisionDetection variable of Rigidbody is equal to **1** or **2**. These values correspond to **Continuous** or **Continuous Dynamic** (see [Unity Manual](https://docs.unity3d.com/Manual/class-Rigidbody.html) for further details).
> - m_EnableBakedLightmaps (into Animator): this parameter is used to detect Sub-optimal, expensive choice of lights, shadows, or reflections smell. If this variable assumes a value greater than **0** the gameobject uses baked lights.
> - m_EnableRealtimeLightmaps (into Animator): this parameter is used to detect Sub-optimal, expensive choice of lights, shadows, or reflections smell. If this variable assumes a value greater than **0** the gameobject uses real-time lights.
> - m_EnableRealtimeLightmaps: this parameter is used to detect the Lack of optimization when drawing/rendering objects. If this variable is present into metadata and it assumes values greater than **0** the smell occurs. Values greater than **0** imply the use of dynamic lights that could influence the rendering.
> - guid: this parameter is used to detect Static coupling smell. It counts the number of guid associated with other gameobjects and verifies that this number is greater that **0**.
> - m_AnyStateTransitions: this variable detects the Usage of anystate in animator controller by checking the presence of m_AnyStateTransitions in the Animator.
> - MeshCollider: this parameter is used to detect Improper mesh settings for a collider by searching for components containing a Meshcollider.
> - num_components: this parameter refers to Bloated Assets smell. The detection rule computes the number of component into metadata and checks if this number is greater than **12** (as defined into the file).
> - TooManyKeyFrames: this parameter is used to detect Too many Keyframes in animations smell. The detection rule checks, into .anim files, if the variable m_Curve has a number of time values greater than **7**.


## Recompiling UnityLint
UnityLint can be recompiled using Visual Studio (tested with Vistual Studio Community Edition 2019 for Windows and for MacOS, version 8.10.21). The sources of the five tools are located into the following folders:
- [ShellStarter](ShellStarter)
- [Analyzer/CSharpAnalyzer](Analyzer/CSharpAnalyzer)
- [Analyzer/CodeSmellAnalyzer](Analyzer/CodeSmellAnalyzer)
- [Analyzer/MetaSmellAnalyzer](Analyzer/MetaSmellAnalyzer)
- [Analyzer/UhityDataAnalyzer](Analyzer/UhityDataAnalyze)

To create a release (as well as to use the ShellStarter), you need to put the compilation result (.exe files and DLLs) of all tools in the same directory. Also, you need to add the smell.txt configuration file.

## License

Distributed under the MIT License. See [LICENSE](LICENSE) for more information.
