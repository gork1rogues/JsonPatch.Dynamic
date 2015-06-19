﻿// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/JsonPatch.Dynamic
//
// Enjoy :-)

using Marvin.JsonPatch.Dynamic.Adapters;
using Marvin.JsonPatch.Dynamic.Converters;
using Marvin.JsonPatch.Dynamic.Helpers;
using Marvin.JsonPatch.Dynamic.Operations;
using Marvin.JsonPatch.Exceptions;
using Marvin.JsonPatch.Operations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Marvin.JsonPatch.Dynamic
{
    
    [JsonConverter(typeof(JsonPatchDocumentConverter))]
    public class JsonPatchDocument : IJsonPatchDocument
    {

        public List<Operation> Operations { get; private set; }

        [JsonIgnore]
        public IContractResolver ContractResolver { get; set; }


        public JsonPatchDocument()
        {
            Operations = new List<Operation>();
            ContractResolver = new DefaultContractResolver();
        }


        // Create from list of operations  
        public JsonPatchDocument(List<Operation> operations)
        {
            Operations = operations;
            ContractResolver = new DefaultContractResolver();
        }

        public JsonPatchDocument Add(string path, object value)
        {
            var checkPathResult = PathHelpers.CheckPath(path);
            if (!checkPathResult.IsCorrectlyFormedPath)
            {
                throw new JsonPatchException(
                   string.Format("Provided string is not a valid path: {0}",
                          path));
            }
 
            Operations.Add(new Operation("add", checkPathResult.AdjustedPath, null, value));
            return this;
        }
          
        public JsonPatchDocument Remove(string path)
        {
            var checkPathResult = PathHelpers.CheckPath(path);
            if (!checkPathResult.IsCorrectlyFormedPath)
            {
                throw new JsonPatchException(
                   string.Format("Provided string is not a valid path: {0}",
                          path));
            }
 
            Operations.Add(new Operation("remove", checkPathResult.AdjustedPath, null, null));
            return this;
        }

        public JsonPatchDocument Replace(string path, object value)
        {
            var checkPathResult = PathHelpers.CheckPath(path);
            if (!checkPathResult.IsCorrectlyFormedPath)
            {
                throw new JsonPatchException(
                   string.Format("Provided string is not a valid path: {0}",
                          path));
            }
 
            Operations.Add(new Operation("replace", checkPathResult.AdjustedPath, null, value));
            return this;
        }
   
        public JsonPatchDocument Move(string from, string path)
        {
            var checkPathResult = PathHelpers.CheckPath(path);
            var checkFromResult = PathHelpers.CheckPath(from);

            if (!checkPathResult.IsCorrectlyFormedPath)
            {
                throw new JsonPatchException(
                   string.Format("Provided string is not a valid path: {0}",
                          path));
            }

            if (!checkFromResult.IsCorrectlyFormedPath)
            {
                throw new JsonPatchException(
                   string.Format("Provided string is not a valid path: {0}",
                          from));
            }


            Operations.Add(new Operation("move", checkPathResult.AdjustedPath, checkFromResult.AdjustedPath));
            return this;
        }
                 
        public JsonPatchDocument Copy(string from, string path)
        {
            var checkPathResult = PathHelpers.CheckPath(path);
            var checkFromResult = PathHelpers.CheckPath(from);

            if (!checkPathResult.IsCorrectlyFormedPath)
            {
                throw new JsonPatchException(
                   string.Format("Provided string is not a valid path: {0}",
                          path));
            }

            if (!checkFromResult.IsCorrectlyFormedPath)
            {
                throw new JsonPatchException(
                   string.Format("Provided string is not a valid path: {0}",
                          from));
            }

            Operations.Add(new Operation("copy", checkPathResult.AdjustedPath, checkFromResult.AdjustedPath));
            return this;
        }



        public void ApplyTo<T>(T objectToApplyTo)
        {
            ApplyTo(objectToApplyTo, new DynamicObjectAdapter());
        }


        /// <summary>
        /// Apply the patch document, passing in a custom IObjectAdapter<typeparamref name=">"/>. 
        /// This method will change the passed-in object.
        /// </summary>
        /// <param name="objectToApplyTo">The object to apply the JsonPatchDocument to</param>
        /// <param name="adapter">The IObjectAdapter instance to use</param>
        public void ApplyTo<T>(T objectToApplyTo, IDynamicObjectAdapter adapter)
        {
            // apply each operation in order
            foreach (var op in Operations)
            {
                op.Apply(objectToApplyTo, adapter);
            }

        }


        // return a copy - original operations should not
        // be editable through this.
        public List<Operation> GetOperations()
        {
            var allOps = new List<Operation>();

            if (Operations != null)
            {
                foreach (var op in Operations)
                {
                    var untypedOp = new Operation();

                    untypedOp.op = op.op;
                    untypedOp.value = op.value;
                    untypedOp.path = op.path;
                    untypedOp.from = op.from;

                    allOps.Add(untypedOp);
                }
            }

            return allOps;
        }
    }
}
