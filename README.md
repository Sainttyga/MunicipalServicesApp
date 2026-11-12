# Municipal Services Application - POE Part 3 (Final)

A C# Windows Forms application for South African residents to **report service delivery issues** (Part 1), **find local events and announcements** (Part 2) and **track their service requests** (Part 3). The Service Request Status feature is built on hand-written trees, a heap and graphs.

**Student:** Lindokuhle Zwane - ST10381088 - AAPD7112

---

## 1. Requirements

| Item | Version |
|---|---|
| Operating system | Windows 10 or 11 |
| SDK | .NET 9.0 SDK (`net9.0-windows`) |
| IDE (optional) | Visual Studio 2022 17.12 or later, with the ".NET desktop development" workload |

The application needs no database or internet connection. Issues and events are saved to `issues_data.json` and `events_data.json` next to the executable. The test project downloads xUnit from NuGet the first time it is restored.

## 2. How to compile
**Visual Studio:** open `MunicipalServicesApp.csproj` and press **Ctrl+Shift+B**.

**Command line** (from the project folder):
```powershell
dotnet build MunicipalServicesApp.csproj -c Release
```
The output is `bin\Release\net9.0-windows\MunicipalServicesApp.exe`.

## 3. How to run
- Visual Studio: press **F5**.
- Command line: `dotnet run --project MunicipalServicesApp.csproj`
- Or double-click `bin\Release\net9.0-windows\MunicipalServicesApp.exe`.

## 4. How to run the unit tests
```powershell
dotnet test Tests\MunicipalServicesApp.Tests
```
The 32 xUnit tests cover every tree, the heap, the graph algorithms, the service request repository, the Part 1 integration and the Part 2 search-history fix. The tested code has no Windows Forms dependency, so the tests also run on Linux or macOS.

## 5. How to use the application

### Main menu
The menu has four buttons: **Report Issues**, **Local Events and Announcements**, **Service Request Status** and **Exit**. Each module opens as a dialog and returns to the menu when it is closed.

### Report Issues (Part 1)
1. Enter the **location** (for example "15 Vilakazi Street, Soweto"), choose a **category**, type a **description** and optionally **attach** a file. The progress bar and messages guide you.
2. Click **Submit**. The confirmation shows the **Issue ID** (for example `3`) and the **Service Request ID** (for example `SR-20261109-0003`). Both numbers work for tracking on the Service Request Status page.

### Local Events and Announcements (Part 2)
- Filter by category or date, or type in the search box. Results filter while you type, but only clicking **Search** is saved to your search history.
- Selecting an event records it as recently viewed.
- The **Recommendations** list is built from your most frequent searches and from high-priority events.

### Service Request Status (Part 3)
| Tab / control | What it does |
|---|---|
| **Track by Request ID** | Type a request ID (`SR-20261109-0003`), your issue number (`3`) or part of an ID (`SR-202611`), then press Enter. The request is selected, a pop-up shows its status, and the blue line reports how many comparisons the AVL tree, the plain BST and a linear scan needed. A partial ID lists the matching requests. |
| **Status / Submitted / Sort by** | Filter by status or by submission period (last 24 hours, 3, 7 or 30 days). Sort by ID (AVL), newest first (red-black) or urgency (min-heap). |
| **All Requests** | Colour-coded list (red = high-priority open, green = resolved). Select a row to see the details, progress bar, status pipeline and timeline. **Simulate Next Status Update** moves it through Received > Under Review > Assigned > In Progress > Resolved. For your own issues, the new status is saved to `issues_data.json`. |
| **Urgent Queue (Min-Heap)** | Open requests by priority, then age, plus the heap array. **Dispatch Most Urgent** takes the root of the heap and advances it. |
| **Related Requests (Graph)** | Graph overview (clusters, most connected requests, strongest links). **Analyse Selected** runs breadth-first search (hops) and depth-first search (cluster order) and suggests requests to batch on the same visit. |
| **Crew Route (MST)** | The cheapest set of roads that links the municipal depot to every area with open requests, with Kruskal and Prim run on the whole road network as a cross-check. |
| **Tree Explorer** | A table of every tree index (nodes, height, rotations). You can draw any tree (general, BST, AVL, red-black) with an in-, pre-, post- or level-order traversal. |

The page starts with 14 demo requests (`SR-yyyyMMdd-90xx`) in Gauteng areas, plus every issue you have reported. The area is detected from the location text (for example "Soweto" or "Sandton"), and the priority comes from the category.

## 6. Data structures and their role

The trees, heap and graph are hand-written in `DataStructures/`. `Services/ServiceRequestRepository.cs` keeps them in step: after every change it rebuilds all indexes from one master list, so they never disagree.

| Structure | File | Role in Service Request Status | Efficiency |
|---|---|---|---|
| **Basic (general) tree** | `GeneralTree.cs` | Groups requests as Root > Category > Status > Request for the Tree Explorer overview (31 nodes, 4 levels with the demo data). | Build O(n), any number of children |
| **Binary tree / Binary search tree** | `BinaryTrees.cs` (`BinarySearchTree`) | Index by request ID, kept unbalanced on purpose as a baseline. IDs start with the date and arrive in date order, so it becomes a chain (height 14 for the 14 demo requests). | O(h): O(log n) for random keys, O(n) for sorted keys |
| **AVL tree** | `BinaryTrees.cs` (`AvlTree`) | Main index for **tracking by unique ID**, the ID-sorted list and partial-ID search. The same 14 IDs give height 4 (10 rotations). | O(log n) search/insert, height < 1.44 log2 n |
| **Red-black tree** | `BinaryTrees.cs` (`RedBlackTree`) | Index by **submission date** (key `yyyyMMddHHmmss\|ID`). Gives the newest-first list and the **Submitted** filter through `Range(low, high)`. | O(log n) insert, range O(log n + k), height ≤ 2 log2(n+1) |
| **Min-heap** | `MinHeap.cs` | **Urgent queue**: highest priority first, then oldest. It is also the priority queue inside Dijkstra and Prim. | Peek O(1), push/pop O(log n), heapify O(n) |
| **Graph** | `WeightedGraph.cs` | (a) Relation graph: requests are vertices, joined when they share an area or category (14 vertices, 17 edges, 4 clusters with the demo data). (b) Road network of 11 areas with distances in km. | Adjacency list, O(V + E) memory |
| **Graph traversal (BFS, DFS)** | `WeightedGraph.cs` | BFS gives the hop distance to related requests. DFS gives the order of a whole cluster and the connected components. | O(V + E) |
| **Dijkstra** | `WeightedGraph.cs` | Real road distances between the areas with open requests. | O((V + E) log V) |
| **Minimum spanning tree (Prim, Kruskal)** | `WeightedGraph.cs` | Cheapest roads joining the depot to every open area (85 km with the demo data). Kruskal and union-find on the whole network give the same 127 km as Prim. | Prim O(E log V), Kruskal O(E log E) |

### Examples
- **Tracking:** the newest demo request takes 4 comparisons in the AVL tree and 14 in the plain BST. With 10 000 requests, the AVL tree would need about 14 comparisons and a chained BST up to 10 000.
- **Date filter:** "Last 7 days" returns 7 of the 14 demo requests after visiting 10 red-black nodes, because subtrees outside the period are skipped.
- **Urgency:** a high-priority Utilities request reported today is dispatched before a low-priority Parks request from last week, because the heap compares priority first and age second.
- **Relationships:** analysing a Soweto request lists the other Soweto requests and the requests in the same category one hop away (BFS), so one crew trip can handle them together.
- **Crew route:** Prim links eight areas with 85 km of road. For example, Depot > Braamfontein > Randburg > Sandton > Alexandra > Midrand, and Depot > Johannesburg CBD > Soweto and Germiston.

`PROJECT_REPORT.md` (section 3) explains each structure in depth.

## 7. Project structure
```
MunicipalServicesApp/
├── Program.cs, Form1.cs              # entry point and main menu
├── ReportIssuesForm.cs                 # Part 1
├── LocalEventsForm.cs                # Part 2
├── ServiceRequestStatusForm.cs       # Part 3 (5 tabs, built in code)
├── Models/                           # Issue (+ JSON storage), Event + EventManager, ServiceRequest
├── Services/ServiceRequestRepository.cs  # owns requests and every index structure
├── DataStructures/                   # GeneralTree, BinaryTrees (BST/AVL/RB), MinHeap, WeightedGraph
├── Tests/MunicipalServicesApp.Tests/ # xUnit tests
└── PROJECT_REPORT.md                 # completion report + technology recommendations + feedback updates

```
