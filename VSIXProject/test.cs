//ThreadHelper.ThrowIfNotOnUIThread();
            string message = "";
            string filePath = "";
            string title = "Command";

            // Getting Development Tools Environment and Hierarchy Items
            //DTE2 applicationObject = GetDTE();
            //UIHierarchy UIH = applicationObject.ToolWindows.SolutionExplorer;
            //UIHierarchyItems hierarchyItems= UIH.UIHierarchyItems;

            
                Array selectedItems = (Array)UIH.SelectedItems;
                if (selectedItems != null)
                {
                    UIHierarchyItem UIHItem = (UIHierarchyItem)selectedItems.GetValue(0);
                    ProjectItem projectItem = UIHItem.Object as ProjectItem;
                    filePath = projectItem.Properties.Item("FullPath").Value.ToString();
                    message = "Filename: " + UIHItem.Name + "\n\n";

                    var componentModel = GetSComponentModel();
                    var workspace = componentModel.GetService<Microsoft.VisualStudio.LanguageServices.VisualStudioWorkspace>();
                    var solution = workspace.CurrentSolution;

                    DocumentId documentId = solution.GetDocumentIdsWithFilePath(filePath).FirstOrDefault();
                    var document = solution.GetDocument(documentId);

                    SemanticModel semanticModel = await document.GetSemanticModelAsync();
                    SyntaxTree tree = await document.GetSyntaxTreeAsync();
                    SyntaxNode root = tree.GetRoot();

                    var members = root.DescendantNodes().OfType<MemberDeclarationSyntax>();

                    foreach (var member in members)
                    {
                        var property = member as PropertyDeclarationSyntax;
                        if (property != null)
                            message += "Property: " +
                                property.Type + " " +
                                property.Identifier + " " +
                                property.AccessorList + "\n";

                        var method = member as MethodDeclarationSyntax;
                        if (method != null)
                            message += "Method: " + method.Identifier + "\n";
                    }
                }
                
		////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		
		
		