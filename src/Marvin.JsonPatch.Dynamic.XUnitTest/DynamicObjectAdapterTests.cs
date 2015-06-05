﻿using Marvin.JsonPatch.Dynamic;
using Marvin.JsonPatch.Dynamic.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Marvin.JsonPatch.Dynamic.XUnitTest
{
    public class DynamicObjectAdapterTests
    {


        [Fact]
        public void AddNewPropertyShouldFailIfRootIsNotAnExpandoObject()
        {

            dynamic doc = new
            {
                Test = 1
            }; 

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add<int>("NewInt", 1);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            Assert.Throws<JsonPatchException>(() => { deserialized.ApplyTo(doc); });
 
        }



        [Fact]
        public void AddNewProperty()
        {

            dynamic obj = new ExpandoObject();
            obj.Test = 1; 

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add<int>("NewInt", 1);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);


            deserialized.ApplyTo(obj);

            Assert.Equal(1, obj.NewInt);
            Assert.Equal(1, obj.Test);


        }



        [Fact]
        public void AddNewPropertyToNestedAnonymousObjectShouldFail()
        {

            dynamic doc = new
            {
                Test = 1,
                nested = new { }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add<int>("Nested/NewInt", 1);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            Assert.Throws<JsonPatchException>(() => { deserialized.ApplyTo(doc); });


        }



        [Fact]
        public void AddNewPropertyToTypedObjectShouldFail()
        {

            dynamic doc = new
            {

                Test = 1,
                nested = new NestedDTO()
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add<int>("Nested/NewInt", 1);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            Assert.Throws<JsonPatchException>(() => { deserialized.ApplyTo(doc); });

        }

         



        [Fact]
        public void AddToExistingPropertyOnNestedObject()
        {


            dynamic doc = new
            {

                Test = 1,
                nested = new NestedDTO()
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("Nested/StringProperty", "A");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);


            deserialized.ApplyTo(doc);

            Assert.Equal("A", doc.nested.StringProperty);
            Assert.Equal(1, doc.Test);

        }


        [Fact]
        public void AddNewPropertyToExpandoOject()
        {

            dynamic doc = new
            {

                Test = 1,
                nested = new ExpandoObject()
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add<int>("Nested/NewInt", 1);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);


            deserialized.ApplyTo(doc);

            Assert.Equal(1, doc.nested.NewInt);
            Assert.Equal(1, doc.Test);
        }




        [Fact]
        public void AddNewPropertyToExpandoOjectInTypedObject()
        {
            var doc = new NestedDTO()
            {
                DynamicProperty = new ExpandoObject()
            };


            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add<int>("DynamicProperty/NewInt", 1);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(1, doc.DynamicProperty.NewInt);

        }



        [Fact]
        public void AddNewPropertyToAnonymousObjectShouldFail()
        {

            dynamic doc = new
            {
                Test = 1
            };

            dynamic valueToAdd = new { IntValue = 1, StringValue = "test", GuidValue = Guid.NewGuid() };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("ComplexProperty", valueToAdd);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
             

            Assert.Throws<JsonPatchException>(() => { deserialized.ApplyTo(doc); });


        }




        [Fact]
        public void AddComplexValueToExpandoObject()
        {


            dynamic doc = new ExpandoObject();
            doc.Test = 1; 

            dynamic valueToAdd = new { IntValue = 1, StringValue = "test", GuidValue = Guid.NewGuid() };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("ComplexProperty", valueToAdd);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);


            deserialized.ApplyTo(doc);

            // check manually - assert can't handle this

            if (!(valueToAdd == doc.ComplexProperty))
            {
                 Assert.Equal(1, 2);
            }
             
            //Assert.Equal(valueToAdd.IntValue, doc.ComplexProperty.IntValue);
            //Assert.Equal(valueToAdd.StringValue, doc.ComplexProperty.StringValue);
            //Assert.Equal(valueToAdd.GuidValue, doc.ComplexProperty.GuidValue);
            Assert.Equal(1, doc.Test);

        }


        [Fact]
        public void AddResultsReplaceShouldFailOnAnonymousDueToNoSetter()
        {
            var doc = new
            {
                StringProperty = "A"
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("StringProperty", "B");


            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("B", doc.StringProperty);

        }



        [Fact]
        public void AddResultsShouldReplace()
        {
            dynamic doc = new ExpandoObject();
            doc.StringProperty = "A"; 

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("StringProperty", "B");


            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("B", doc.StringProperty);

        }


        [Fact]
        public void AddResultsShouldReplaceInNested()
        {
            dynamic doc = new ExpandoObject();
            doc.InBetweenFirst.InBetweenSecond.StringProperty = "A";

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("/InBetweenFirst/InBetweenSecond/StringProperty", "B");


            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("B", doc.InBetweenFirst.InBetweenSecond.StringProperty);

        }


        [Fact]
        public void AddResultsShouldReplaceInNestedInDynamic()
        {
            dynamic doc = new ExpandoObject();
            doc.Nested = new NestedDTO();
            doc.Nested.DynamicProperty = new ExpandoObject();
            doc.Nested.DynamicProperty.InBetweenFirst = new ExpandoObject();
            doc.Nested.DynamicProperty.InBetweenFirst.InBetweenSecond = new ExpandoObject();
            doc.Nested.DynamicProperty.InBetweenFirst.InBetweenSecond.StringProperty = "A";
           

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("/Nested/DynamicProperty/InBetweenFirst/InBetweenSecond/StringProperty", "B");


            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("B", doc.Nested.DynamicProperty.InBetweenFirst.InBetweenSecond.StringProperty);

        }




        [Fact]
        public void ShouldNotBeAbleToAddToNonExistingPropertyThatIsNotTheRoot()
        {
            //Adding to a Nonexistent Target
            //
            //   An example target JSON document:
            //   { "foo": "bar" }
            //   A JSON Patch document:
            //   [
            //        { "op": "add", "path": "/baz/bat", "value": "qux" }
            //      ]
            //   This JSON Patch document, applied to the target JSON document above,
            //   would result in an error (therefore, it would not be applied),
            //   because the "add" operation's target location that references neither
            //   the root of the document, nor a member of an existing object, nor a
            //   member of an existing array.


            var doc = new NestedDTO()
            {
                DynamicProperty = new ExpandoObject()
            };


            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("DynamicProperty/OtherProperty/IntProperty", 1);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);


            Assert.Throws<JsonPatchException>(() => { deserialized.ApplyTo(doc); });

        }




        [Fact]
        public void ShouldNotBeAbleToAddToNonExistingPropertyInNestedPropertyThatIsNotTheRoot()
        {
            //Adding to a Nonexistent Target
            //
            //   An example target JSON document:
            //   { "foo": "bar" }
            //   A JSON Patch document:
            //   [
            //        { "op": "add", "path": "/baz/bat", "value": "qux" }
            //      ]
            //   This JSON Patch document, applied to the target JSON document above,
            //   would result in an error (therefore, it would not be applied),
            //   because the "add" operation's target location that references neither
            //   the root of the document, nor a member of an existing object, nor a
            //   member of an existing array.


            var doc = new
            {
                Foo = "bar"
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("baz/bat", "qux");


            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);


            Assert.Throws<JsonPatchException>(() => { deserialized.ApplyTo(doc); });

        }



        [Fact]
        public void ShouldReplacePropertyWithDifferentCase()
        {
            var doc = new
            {
                StringProperty = "A"
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("stringproperty", "B");


            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("B", doc.StringProperty);

        }




        [Fact]
        public void AddToList()
        {
            var doc = new
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.AddToArray("IntegerList", 4, 0);


            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);


            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, doc.IntegerList);
        }


        [Fact]
        public void ShouldAddToListWithDifferentCase()
        {
            var doc = new
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.AddToArray("integerlist", 4, 0);


            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);


            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, doc.IntegerList);
        }




        [Fact]
        public void AddToListInvalidPositionTooLarge()
        {
            var doc = new
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.AddToArray("IntegerList", 4, 4);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);


            Assert.Throws<JsonPatchException>(() => { deserialized.ApplyTo(doc); });
        }





        [Fact]
        public void AddToListAtEndWithSerialization()
        {
            var doc = new
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.AddToArray("IntegerList", 4, 3);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 1, 2, 3, 4 }, doc.IntegerList);

        }





        [Fact]
        public void AddToListAtBeginning()
        {
            var doc = new
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.AddToArray("IntegerList", 4, 0);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, doc.IntegerList);


        }


        [Fact]
        public void AddToListInvalidPositionTooSmall()
        {

            var doc = new
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.AddToArray("IntegerList", 4, -1);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            Assert.Throws<JsonPatchException>(() => { deserialized.ApplyTo(doc); });

        }




        [Fact]
        public void AddToListAppend()
        {
            var doc = new
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.AddToArray("IntegerList", 4);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 1, 2, 3, 4 }, doc.IntegerList);

        }

    }
}
