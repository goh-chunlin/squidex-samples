﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Squidex.CLI.Commands.Implementation.FileSystem.Emedded
{
    public sealed class EmbeddedFileSystem : IFileSystem
    {
        private readonly Assembly assembly;
        private readonly string assemblyPath;

        public string FullName { get; }

        public bool CanWrite => false;

        public EmbeddedFileSystem(Assembly assembly, string assemblyPath)
        {
            this.assembly = assembly;
            this.assemblyPath = assemblyPath;

            FullName = $"{assembly.FullName}/{assemblyPath}";
        }

        public IFile GetFile(FilePath path)
        {
            var relativePath = GetRelativePath(path);

            return new EmbeddedFile(assembly, path.Elements.Last(), relativePath);
        }

        public IEnumerable<IFile> GetFiles(FilePath path, string extension)
        {
            var relativePath = GetRelativePath(path);

            foreach (var fullName in assembly.GetManifestResourceNames())
            {
                if (fullName.StartsWith(relativePath, StringComparison.OrdinalIgnoreCase) && fullName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    var segments = fullName.Split('.');

                    var name = string.Join('.', segments.TakeLast(2));

                    yield return new EmbeddedFile(assembly, name, fullName);
                }
            }
        }

        private string GetRelativePath(FilePath path)
        {
            return string.Join('.', Enumerable.Repeat(assemblyPath, 1).Concat(path.Elements));
        }

        public void Dispose()
        {
        }
    }
}
