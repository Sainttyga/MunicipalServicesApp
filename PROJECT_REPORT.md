# Municipal Services Application - Project Completion Report

**Student:** Lindokuhle Zwane (ST10381088)  **Module:** AAPD7112  **Submission:** POE Part 3 (final)

## 1. Project Overview

The Municipal Services Application helps South African residents deal with their municipality from one desktop application. It was built in three parts over the semester. Each part added a module and reused what came before.

| Part | Module | What the resident can do | Main structures and techniques |
|---|---|---|---|
| 1 | Report Issues | Report a problem (location, category, description, attachment) and receive an issue number | `List<Issue>` saved to `issues_data.json` with System.Text.Json, validation, progress bar and engagement messages |
| 2 | Local Events and Announcements | Filter and search events, see recommendations based on search history | `SortedDictionary`, `Dictionary`, `HashSet`, `Queue`, `Stack`, priority queue (`SortedList`) |
| 3 | Service Request Status | Track a request by its unique ID, see its progress, and see what the municipality will do next | General tree, BST, AVL tree, red-black tree, min-heap, weighted graph, BFS, DFS, Dijkstra, Prim, Kruskal |

The modules are connected. An issue reported in Part 1 becomes a tracked service request in Part 3 (ID `SR-{date}-{issue number}`), and the resident can track it with either number. Status changes are written back to the issue and saved to `issues_data.json`, so they survive a restart.

### 1.1 How Service Request Status meets the brief
- **Well-organised list with status:** every request is listed with category, area, priority, status, date and progress. It can be sorted by ID (AVL in-order), newest first (red-black in-order) or urgency (heap order). It can be filtered by status and by submission period. Rows are colour-coded, and the selected request shows a progress bar, the status pipeline and a dated timeline.
- **Tracking by unique identifier:** the request ID (or the issue number) is looked up in an AVL tree in O(log n). The screen reports the comparisons needed by the AVL tree, the plain BST and a linear scan, so the efficiency is visible. A partial ID returns suggestions through an AVL range query.
- **Advanced data structures:** each structure drives a visible feature (section 3). None of them is only for decoration.

## 2. Development Journey by Part

### Part 1 - Report Issues
I built the main menu and the Report Issues form with Windows Forms on .NET 9. Each issue gets a sequential ID and is saved to a JSON file, so reports survive a restart. To keep residents engaged, I added a progress bar with changing messages and a confirmation with the issue number.

### Part 2 - Local Events and Announcements
I added an `EventManager` with several data structures:
- a `SortedDictionary` keeps events in date order;
- `Dictionary` indexes support lookup by ID and by category, and `HashSet`s hold the unique categories and dates;
- a `Stack` keeps recently viewed events, and a `Queue` keeps upcoming events;
- a `SortedList` priority queue holds high-priority events.

Search frequencies are counted in a dictionary to recommend events.

### Part 3 - Service Request Status
I implemented the trees, heap and graph myself in `DataStructures/`. A `ServiceRequestRepository` owns every request and keeps all the indexes in step. A five-tab form presents the result, and an xUnit test project checks every structure automatically.

## 3. Data Structures in the Service Request Status Feature

The demo data holds 14 requests across Gauteng (Soweto, Sandton, Randburg, Alexandra and others), using the same categories as Report Issues, plus every issue reported in Part 1. The figures below are for the 14 demo requests. After any change, the repository rebuilds every index from one master list, so the structures never disagree.

### 3.1 Basic (general) tree - `GeneralTreeNode<T>`
- **What it is:** a tree where a node may have any number of children.
- **Role:** groups requests as *Root > Category > Status > Request* for the Tree Explorer overview, so a manager sees at a glance, for example, that Roads has one resolved and two open requests.
- **Efficiency:** building is O(n). Adding a new category just adds a branch, which a binary tree cannot model naturally. With the demo data the tree has 31 nodes and 4 levels.

### 3.2 Binary tree and binary search tree - `BinarySearchTree<TKey,TValue>`
- **What it is:** a binary tree (at most two children per node) where smaller keys go left and larger keys go right. It supports in-order, pre-order, post-order and level-order traversals, plus Hibbard deletion.
- **Role:** a baseline index by request ID that shows why balancing is needed. Request IDs embed the date (`SR-20261109-…`) and requests arrive in date order, so every new key is the largest and the BST becomes a chain. With 14 requests its height is 14, so finding the newest request takes 14 comparisons.
- **Efficiency:** O(h) per operation. That is O(log n) on random keys but O(n) on the sorted keys this application actually receives.

### 3.3 AVL tree - `AvlTree<TKey,TValue>`
- **What it is:** a BST where the two subtrees of every node differ in height by at most one. After each insert, the LL, RR, LR and RL cases are fixed with single or double rotations.
- **Role:** the main index for **tracking by unique ID**. It also provides the ID-ordered list and the prefix search (`Range("SR-2026", "SR-2026\uffff")`).
- **Example:** the same 14 sorted IDs give an AVL tree of height 4 (10 rotations while building), so any lookup needs at most 4 comparisons. With 10 000 requests, the AVL tree would need about 14 comparisons where the chained BST would need up to 10 000.
- **Efficiency:** search and insert are O(log n), and the height stays below 1.44 log2(n + 2).

### 3.4 Red-black tree - `RedBlackTree<TKey,TValue>`
- **What it is:** a self-balancing BST (left-leaning variant). The root is black, a red node never has a red child, and every path has the same number of black nodes. These rules keep the height at most 2 log2(n + 1). `IsValid()` checks every rule.
- **Role:** the index by **submission date** (key = timestamp + ID). It gives the newest-first list and powers the **Submitted** filter through a range query. The range query skips subtrees outside the period, so it visits O(log n + k) nodes for k results.
- **Example:** "Last 7 days" returns 7 requests after visiting 10 of the 14 nodes. A unit test shows that a 10-key range in a 1 000-key tree visits fewer than 50 nodes and matches a linear scan exactly.
- **Why both AVL and red-black:** the red-black tree is less strictly balanced (height 4, black height 3 here). In exchange, it does less rebalancing per insert. That suits the date index, which receives a new largest key with every request.

### 3.5 Min-heap - `MinHeap<T>`
- **What it is:** a complete binary tree stored in an array (`parent = (i-1)/2`), where every parent is no larger than its children.
- **Role:** the **Urgent Queue**. Open requests are compared by priority (High, Medium, Low) and then by age, so the root is always the next job to dispatch. **Dispatch Most Urgent** takes the root and advances it. The same heap is the priority queue inside Dijkstra and Prim.
- **Efficiency:** peek is O(1), push and pop are O(log n), and building from all open requests is O(n) with bottom-up heapify. Sorting every request just to find the next job would cost O(n log n) each time.

### 3.6 Graphs and graph traversal - `WeightedGraph<T>`
- **What it is:** vertices and weighted edges in adjacency lists (memory O(V + E)).
  - Breadth-first search uses a queue and visits vertices level by level.
  - Depth-first search uses a stack and follows one branch as far as it goes.
- **Role 1 - relation graph:** each request is a vertex, with edges between requests in the same area (weight 2), the same category (3) or both (1).
  - BFS from a request gives every related request and its hop distance.
  - DFS gives the visiting order of the whole cluster and the connected components.
  - The demo data forms 14 vertices, 17 edges and 4 clusters. Selecting a Soweto request shows the other Soweto requests one hop away, so one crew trip can handle them together.
- **Role 2 - road network:** 11 areas (including the municipal depot) joined by 16 roads with distances in km.

### 3.7 Shortest paths and minimum spanning trees - Dijkstra, Prim, Kruskal
- **What they are:**
  - Dijkstra finds the shortest distance from one vertex to all others.
  - A minimum spanning tree connects every vertex with the smallest total weight and no cycles.
  - Prim grows the tree from a start vertex using the heap.
  - Kruskal sorts all edges and joins separate components with union-find (path compression and union by rank).
- **Role:** the **Crew Route** tab first finds the areas that have open requests. Dijkstra gives the real road distance between each pair, and Prim then finds the cheapest network linking the depot to all of them.
- **Example:** with open requests in eight areas, the plan needs **85 km**, for example Depot > Braamfontein > Randburg > Sandton > Alexandra > Midrand. On the whole road network, Kruskal and Prim both select 10 roads totalling **127 km**. This cross-check runs in the unit tests, together with Prim vs Kruskal on 20 random graphs and Dijkstra vs Floyd-Warshall.
- **Efficiency:** Dijkstra is O((V + E) log V), Prim is O(E log V) and Kruskal is O(E log E).

### 3.8 Complexity summary

| Structure / algorithm | Search | Insert | Other | Used for |
|---|---|---|---|---|
| General tree | O(n) | O(1) as a child | Height 4 | Category overview |
| BST (unbalanced) | O(n) worst | O(n) worst | Height 14 on sorted IDs | Baseline comparison |
| AVL tree | O(log n) | O(log n) | Range O(log n + k) | Tracking by ID, prefix search |
| Red-black tree | O(log n) | O(log n), fewer rebalances | Range O(log n + k) | Date order, Submitted filter |
| Min-heap | Peek O(1) | O(log n) | Pop O(log n), heapify O(n) | Urgent queue, Dijkstra, Prim |
| BFS / DFS | O(V + E) | - | - | Related requests, clusters |
| Dijkstra | O((V + E) log V) | - | - | Road distances |
| Prim / Kruskal | O(E log V) / O(E log E) | - | - | Crew route |

## 4. Challenges and How They Were Overcome

### Part 1
- **Keeping reports after the application closes.** I saved issues to a JSON file with System.Text.Json and reload them on start-up.
- **Validation that does not frustrate.** Each required field is checked in turn, and the message names the missing field.

### Part 2
- **Fast search by date and category.** Events are indexed in a `SortedDictionary` by date and in a `Dictionary` by category, so each kind of search starts from the right index instead of scanning a list.
- **Search history that told the truth.**
  - **Problem:** while preparing Part 3, I found two bugs. The search box recorded a search on every keystroke, and building the recommendation list recorded the top searches again.
  - **Solution:** only submitted searches are recorded now, and a unit test checks the counts.

### Part 3
- **Getting the balancing right.**
  - **Problem:** rotations are easy to get wrong.
  - **Solution:** the AVL tree caches node heights and handles the four cases with two rotation methods. I chose the left-leaning red-black variant because it needs only rotate-left, rotate-right and a colour flip. `IsBalanced()` and `IsValid()` check the invariants, and the tests insert sorted keys (the worst case), random keys and 2 000 random values.
- **Discovering that my own IDs break a plain BST.**
  - **Problem:** the request IDs start with the date, so they always arrive in increasing order.
  - **Solution:** I kept the unbalanced BST on purpose and show its height (14) next to the AVL tree (4) and the comparison counts. This turned a bug into the clearest evidence of why balanced trees matter.
- **Making every structure useful.**
  - **Problem:** structures added only to tick a box would add nothing for residents or staff.
  - **Solution:** each structure drives a feature: tracking, date filtering, dispatching, related requests and crew routes. The screens show evidence such as heights, rotations, nodes visited, comparisons and kilometres.
- **A minimum spanning tree only connects the vertices it is given.**
  - **Problem:** the areas with open requests are usually not joined directly by a road.
  - **Solution:** I run Dijkstra between every pair of those areas to build a complete distance graph, run Prim on that, and expand each link back into the real roads.
- **Keeping several indexes consistent.**
  - **Problem:** six structures had to agree after every status change.
  - **Solution:** a single master list is the source of truth, and `Rebuild()` recreates every index after each change. With municipal volumes this costs milliseconds and removes a whole class of bugs.
- **Static field order.**
  - **Problem:** the road network was built from the area list before that list was initialised, which crashed the repository's static constructor.
  - **Solution:** the unit tests caught it, and I moved the declaration (C# initialises static fields in textual order).
- **Integrating with Parts 1 and 2.**
  - **Problem:** Part 1 issues use a plain number and a saved status ("Pending"), while Part 3 needs a unique ID and a status pipeline.
  - **Solution:** issues are imported into the status page without duplicates (tracked in a `HashSet`). Their saved status is restored into the pipeline, and status changes are saved back to `issues_data.json`.
- **Testing without clicking through the UI every time.**
  - **Solution:** I separated the logic from the forms, so the structures compile into an xUnit project. The 32 tests compare the structures with trusted methods (linear scans, Floyd-Warshall, Prim vs Kruskal), and they run on any operating system.

## 5. Key Learnings

- **Choose the structure from the access pattern.** "Find by ID", "show by date", "what next?" and "what is related?" are different questions and need different structures. Seeing my BST reach height 14 on my own data, while the AVL tree stayed at 4, showed me why balanced trees exist.
- **Algorithms in practice.** I now understand rotations and invariants in AVL and red-black trees, sifting in a binary heap, why BFS needs a queue and DFS a stack, how Dijkstra relies on a priority queue, and how Prim and Kruskal reach the same minimum cost by different routes. Getting 127 km from both was a satisfying correctness check.
- **Range queries and pruning.** The same BST ordering that makes lookups fast also makes "everything between A and B" fast, if the search skips subtrees that cannot match.
- **Design.** Separating the model (`ServiceRequest`), the data layer (`ServiceRequestRepository`, `EventManager`) and the screens made the code easier to test, to change and to explain. Keeping one source of truth and rebuilding derived indexes is a simple, reliable pattern.
- **Testing as a problem-solving tool.** Writing tests that compare against a trusted method found real bugs early, for example the static initialisation order. They also gave me confidence to refactor.
- **User experience.** Showing *why* (comparison counts, nodes visited, recommendation reasons, timelines) makes the application easier to trust. Tabs and pop-ups keep each screen focused.
- **Working from feedback.** Turning the lecturer's comments into specific changes and recording them (section 9) improved the whole application, not just Part 3.

## 6. Technology Recommendations

| # | Technology | What it adds | Compatibility with this project |
|---|---|---|---|
| 1 | **SQL Server / SQLite with Entity Framework Core** | Replaces the JSON files: safe concurrent access, queries and many users | EF Core is the standard .NET data library. `IssueDataStorage`, `EventDataStorage` and `ServiceRequestRepository` are the only classes that store data, so only they change. |
| 2 | **Target .NET 10 (LTS)** | Long-term support (3 years) instead of .NET 9's shorter support window, plus performance improvements | Change `net9.0-windows` to `net10.0-windows`. The code uses only standard Windows Forms and base-library APIs. |
| 3 | **ASP.NET Core Web API + SignalR** | Staff and residents share one live system, and status changes are pushed to residents instantly | Same C# language and models. The data structures can be reused on the server. |
| 4 | **.NET MAUI mobile app** | Residents report issues from their phones with a photo and GPS location | Shares the C# models and logic with the desktop app. |
| 5 | **Mapping/GIS (OpenStreetMap, Google Maps Platform)** | Real coordinates and road distances instead of the demo road graph | `WeightedGraph<T>` works with any vertex type, so Dijkstra and the MST run unchanged on real road data. |
| 6 | **SMS / WhatsApp / e-mail notifications (Clickatell, Twilio, SendGrid)** | Residents are told when their request changes status, even without data or a smartphone | All status changes go through `ServiceRequest.Advance`, so there is one place to send the notification. |
| 7 | **Authentication (ASP.NET Core Identity / Azure AD B2C)** | Residents see only their own requests, and only staff can advance statuses | Standard for .NET applications, and required by POPIA before storing personal data. |
| 8 | **CI with GitHub Actions** | Every push is built and tested automatically | The xUnit project already runs with `dotnet test` on Linux runners. |
| 9 | **ML.NET** | Learns request priority and recommendation weights from real history | Native C# library that runs inside the application. |

## 7. Justification of the Recommendations

- **Database (1):** JSON files work for one user on one PC, but they cannot be shared safely and are rewritten completely on every save. Because every request passes through one repository class, adding EF Core is a contained change. The in-memory trees and heap can remain as fast caches on top of the database.
- **.NET 10 LTS (2):** .NET 9 is a standard-term release with a short support window. Software that holds residents' personal information should run on a long-term-support version that receives security patches. The upgrade costs one line in the project file.
- **Web API, SignalR and mobile (3, 4):** most South African residents use phones rather than desktop PCs. A shared C# back end means the logic is written once. Live updates make "track my request" genuinely useful instead of something residents have to keep checking.
- **Mapping (5):** the crew-route feature is the most valuable graph application here, and the demo road graph is its main limitation. Real road data lets the existing algorithms plan real routes and save fuel and crew time, with no algorithm changes.
- **Notifications (6):** SMS reaches residents without smartphones or data, which matters for inclusive service delivery in South Africa. The single status-change method makes the integration simple.
- **Authentication (7):** separating resident and staff actions is necessary before going live, and the Protection of Personal Information Act (POPIA) requires protecting residents' data.
- **CI (8):** the data structures are pure logic and cheap to test. Automatic builds protect them when the code changes, at no extra cost for public repositories.
- **ML.NET (9):** the priority and recommendation weights are hand-tuned today. Training them on real usage would improve dispatch order and event suggestions without adding another language or service.

## 8. Testing

- `dotnet test Tests\MunicipalServicesApp.Tests` runs 32 xUnit tests, and all of them pass:
  - **Trees:** sorted and random inserts, duplicates, all four traversals, the four AVL rotation cases, red-black invariants on 2 000 random keys, BST deletion, and range queries against linear scans.
  - **Heap:** ordering, heapify, and empty-heap errors.
  - **Graph:** BFS hop counts, DFS and components, Dijkstra vs Floyd-Warshall, Prim vs Kruskal on 20 random graphs, and rejection of negative weights.
  - **Request repository:** indexing, tracking cost, prefix search, date range, heap order, dispatch, relations, the crew route (127 km network), Part 1 import, tracking by issue number, and restoring a saved status.
  - **Part 2:** a regression test showing that recommendations no longer inflate the search history.
- The Windows Forms screens were checked by running each tab with the demo data.

## 9. Updates Based on Lecturer Feedback

The changes below were made to the Part 1 and Part 2 work for the final submission. Each entry gives the feedback or problem behind the change and where it is in the code.

| Part | Feedback / issue found | Status |
|---|---|---|
| Part 2 | "Improve interface integration, search-history analysis and recommendation ranking to enhance functionality." | Addressed (below) |
| Part 1 | A reported issue received an ID, but there was nowhere to follow its progress. | Addressed in Part 3 |
| Parts 1-2 | The Service Request Status button was disabled ("Coming Soon"). | Addressed |

### 9.1 Interface integration
- **Service Request Status is live.** The main-menu button is enabled, styled like the other buttons, and opens `ServiceRequestStatusForm` as a dialog (`Form1.btnServiceStatus_Click`).
- **Report Issues feeds the status page.** Every saved `Issue` becomes a `ServiceRequest` with the ID `SR-{date}-{issue number}`. The area is detected from the location text and the priority from the category. The submit confirmation now shows this request ID and explains how to track it.
- **Track with either number.** Residents can track with the request ID or with the issue number they were given in Part 1.
- **Status is saved back to Part 1.** When a request advances, `Issue.Status` changes from "Pending" to "Under Review", "Assigned" and so on, and `issues_data.json` is saved. The status therefore survives a restart. On start-up, the saved status is restored into the request's pipeline.

### 9.2 Search-history analysis
- **Problem 1:** the search box recorded a search on **every keystroke**. Typing "water" saved "w", "wa", "wat", "wate" and "water", so the history and the frequency counts were wrong. **Fix:** results still filter while typing, but only clicking **Search** records the search (`EventManager.SearchEvents(keyword, recordSearch)`, `LocalEventsForm.PerformSearch(recordSearch)`).
- **Problem 2:** building the recommendation list called `SearchEvents`, which recorded the top searches again, so every refresh inflated their counts. **Fix:** recommendations now search without recording. A unit test (`BuildingRecommendationsDoesNotInflateSearchHistory`) proves the counts stay correct.

### 9.3 Recommendation ranking
- With correct frequencies, the most-searched categories and keywords really are ranked first, followed by high-priority events (priority queue). Previously, repeated refreshes and partial words could push unrelated events to the top.
