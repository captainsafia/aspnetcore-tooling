﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Razor.Workspaces;
using Microsoft.VisualStudio.Text;
using Moq;
using Xunit;

namespace Microsoft.VisualStudio.LanguageServerClient.Razor
{
    public class CSharpVirtualDocumentPublisherTest
    {
        [Fact]
        public void DocumentManager_Changed_Added_Noops()
        {
            // Arrange
            var fileInfoProvider = new Mock<RazorDynamicFileInfoProvider>(MockBehavior.Strict);
            var publisher = new CSharpVirtualDocumentPublisher(fileInfoProvider.Object);
            var args = new LSPDocumentChangeEventArgs(old: null, @new: Mock.Of<LSPDocumentSnapshot>(), LSPDocumentChangeKind.Added);

            // Act & Assert
            publisher.DocumentManager_Changed(sender: null, args);
        }

        [Fact]
        public void DocumentManager_Changed_Removed_Noops()
        {
            // Arrange
            var fileInfoProvider = new Mock<RazorDynamicFileInfoProvider>(MockBehavior.Strict);
            var publisher = new CSharpVirtualDocumentPublisher(fileInfoProvider.Object);
            var args = new LSPDocumentChangeEventArgs(old: Mock.Of<LSPDocumentSnapshot>(), @new: null, LSPDocumentChangeKind.Removed);

            // Act & Assert
            publisher.DocumentManager_Changed(sender: null, args);
        }

        [Fact]
        public void DocumentManager_Changed_VirtualDocumentChanged_NonCSharp_Noops()
        {
            // Arrange
            var fileInfoProvider = new Mock<RazorDynamicFileInfoProvider>(MockBehavior.Strict);
            var publisher = new CSharpVirtualDocumentPublisher(fileInfoProvider.Object);
            var args = new LSPDocumentChangeEventArgs(
                old: Mock.Of<LSPDocumentSnapshot>(), @new: Mock.Of<LSPDocumentSnapshot>(),
                virtualOld: Mock.Of<VirtualDocumentSnapshot>(), virtualNew: Mock.Of<VirtualDocumentSnapshot>(),
                LSPDocumentChangeKind.VirtualDocumentChanged);

            // Act & Assert
            publisher.DocumentManager_Changed(sender: null, args);
        }

        [Fact]
        public void DocumentManager_Changed_VirtualDocumentChanged_UpdatesFileInfo()
        {
            // Arrange
            var csharpSnapshot = new CSharpVirtualDocumentSnapshot(new Uri("C:/path/to/something.razor.g.cs"), Mock.Of<ITextSnapshot>(), hostDocumentSyncVersion: 1337);
            var lspDocument = new TestLSPDocumentSnapshot(new Uri("C:/path/to/something.razor"), 1337, csharpSnapshot);
            var fileInfoProvider = new Mock<RazorDynamicFileInfoProvider>(MockBehavior.Strict);
            fileInfoProvider.Setup(provider => provider.UpdateLSPFileInfo(lspDocument.Uri, It.IsAny<DynamicDocumentContainer>()))
                .Verifiable();
            var publisher = new CSharpVirtualDocumentPublisher(fileInfoProvider.Object);
            var args = new LSPDocumentChangeEventArgs(
                old: Mock.Of<LSPDocumentSnapshot>(), @new: lspDocument,
                virtualOld: Mock.Of<VirtualDocumentSnapshot>(), virtualNew: csharpSnapshot,
                LSPDocumentChangeKind.VirtualDocumentChanged);

            // Act
            publisher.DocumentManager_Changed(sender: null, args);

            // Assert
            fileInfoProvider.VerifyAll();
        }
    }
}
