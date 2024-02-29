/// <summary> 
/// Authors:   Joe Zachary
///            Daniel Kopta
///            Jim de St. Germain
/// Date:      Updated Spring 2022 
/// Course:    CS 3500, University of Utah, School of Computing 
/// Copyright: CS 3500 - This work may not be copied for use 
///                      in Academic Coursework.  See below. 
/// 
/// File Contents 
///
///   This file contains proprietary grading tests for CS 3500.  These tests cases
///   are for individual student use only and MAY NOT BE SHARED.  Do not back them up
///   nor place them in any online repository.  Improper use of these test cases
///   can result in removal from the course and an academic misconduct sanction.
///   
///   These tests are for your private use only to improve the quality of the
///   rest of your assignments
/// </summary>

using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using SpreadsheetUtilities;
using System.Threading;
using System.Xml;

namespace AS5_Grading_Tests
{

    /// <summary>
    ///This is a test class for SpreadsheetTest and is intended
    ///to contain all SpreadsheetTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AS5_Full_Spreadsheet_Grading_Tests
    {

        // Verifies cells and their values, which must alternate.
        public void VV(AbstractSpreadsheet sheet, params object[] constraints)
        {
            for (int i = 0; i < constraints.Length; i += 2)
            {
                if (constraints[i + 1] is double)
                {
                    Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                }
                else
                {
                    Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                }
            }
        }


        // For setting a spreadsheet cell.
        public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
        {
            List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
            return result;
        }

        // Tests IsValid
        [TestMethod, Timeout(2000)]
        [TestCategory("1")]
        public void IsValidTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "x");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void IsValidTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("A1", "x");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("3")]
        public void IsValidTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("4")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IsValidTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("B1", "= A1 + C1");
        }

        // Tests Normalize
        [TestMethod, Timeout(2000)]
        [TestCategory("5")]
        public void NormalizeTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("", s.GetCellContents("b1"));
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("6")]
        public void NormalizeTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("hello", ss.GetCellContents("b1"));
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("7")]
        public void NormalizeTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("A1", "6");
            s.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("8")]
        public void NormalizeTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("a1", "5");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
        }

        // Simple tests
        [TestMethod, Timeout(2000)]
        [TestCategory("9")]
        public void EmptySheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            VV(ss, "A1", "");
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("10")]
        public void OneString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneString(ss);
        }

        public void OneString(AbstractSpreadsheet ss)
        {
            Set(ss, "B1", "hello");
            VV(ss, "B1", "hello");
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("11")]
        public void OneNumber()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneNumber(ss);
        }

        public void OneNumber(AbstractSpreadsheet ss)
        {
            Set(ss, "C1", "17.5");
            VV(ss, "C1", 17.5);
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("12")]
        public void OneFormula()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneFormula(ss);
        }

        public void OneFormula(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "5.2");
            Set(ss, "C1", "= A1+B1");
            VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("13")]
        public void ChangedAfterModify()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            Set(ss, "C1", "17.5");
            Assert.IsTrue(ss.Changed);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("13b")]
        public void UnChangedAfterSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "C1", "17.5");
            ss.Save("changed.txt");
            Assert.IsFalse(ss.Changed);
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("14")]
        public void DivisionByZero1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero1(ss);
        }

        public void DivisionByZero1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "0.0");
            Set(ss, "C1", "= A1 / B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("15")]
        public void DivisionByZero2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero2(ss);
        }

        public void DivisionByZero2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "5.0");
            Set(ss, "A3", "= A1 / 0.0");
            Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
        }



        [TestMethod, Timeout(2000)]
        [TestCategory("16")]
        public void EmptyArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            EmptyArgument(ss);
        }

        public void EmptyArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("17")]
        public void StringArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            StringArgument(ss);
        }

        public void StringArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "hello");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("18")]
        public void ErrorArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ErrorArgument(ss);
        }

        public void ErrorArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= C1");
            Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("19")]
        public void NumberFormula1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula1(ss);
        }

        public void NumberFormula1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + 4.2");
            VV(ss, "C1", 8.3);
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("20")]
        public void NumberFormula2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula2(ss);
        }

        public void NumberFormula2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "= 4.6");
            VV(ss, "A1", 4.6);
        }


        // Repeats the simple tests all together
        [TestMethod, Timeout(2000)]
        [TestCategory("21")]
        public void RepeatSimpleTests()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "17.32");
            Set(ss, "B1", "This is a test");
            Set(ss, "C1", "= A1+B1");
            OneString(ss);
            OneNumber(ss);
            OneFormula(ss);
            DivisionByZero1(ss);
            DivisionByZero2(ss);
            StringArgument(ss);
            ErrorArgument(ss);
            NumberFormula1(ss);
            NumberFormula2(ss);
        }

        // Four kinds of formulas
        [TestMethod, Timeout(2000)]
        [TestCategory("22")]
        public void Formulas()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formulas(ss);
        }

        public void Formulas(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.4");
            Set(ss, "B1", "2.2");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= A1 - B1");
            Set(ss, "E1", "= A1 * B1");
            Set(ss, "F1", "= A1 / B1");
            VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("23")]
        public void Formulasa()
        {
            Formulas();
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("24")]
        public void Formulasb()
        {
            Formulas();
        }


        // Are multiple spreadsheets supported?
        [TestMethod, Timeout(2000)]
        [TestCategory("25")]
        public void Multiple()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            AbstractSpreadsheet s2 = new Spreadsheet();
            Set(s1, "X1", "hello");
            Set(s2, "X1", "goodbye");
            VV(s1, "X1", "hello");
            VV(s2, "X1", "goodbye");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("26")]
        public void Multiplea()
        {
            Multiple();
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("27")]
        public void Multipleb()
        {
            Multiple();
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("28")]
        public void Multiplec()
        {
            Multiple();
        }

        // Reading/writing spreadsheets
        [TestMethod, Timeout(2000)]
        [TestCategory("29")]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveToMissingFolderTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("\\missing\\saveme.txt");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("30")]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadFromMissingFileTest()
        {
            // should not be able to read 
            AbstractSpreadsheet ss = new Spreadsheet("q:\\missing\\save.txt", s => true, s => s, "");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("31")]
        public void SaveThenReadTest1()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            Set(s1, "A1", "hello");
            s1.Save("save1.txt");
            s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
            Assert.AreEqual("hello", s1.GetCellContents("A1"));
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("32")]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadInvalidXMLFileTest()
        {
            using (StreamWriter writer = new StreamWriter("save2.txt"))
            {
                writer.WriteLine("This");
                writer.WriteLine("is");
                writer.WriteLine("a");
                writer.WriteLine("test!");
            }
            AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("33")]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ReadFileInvalidVersionTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("save3.txt");
            ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("34")]
        public void VersionSavedCorrectlyTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
            ss.Save("save4.txt");
            Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("35")]
        public void SaveThenReadTest2()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            using (XmlWriter writer = XmlWriter.Create("save5.txt", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("contents", "5.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A3");
                writer.WriteElementString("contents", "4.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A4");
                writer.WriteElementString("contents", "= A2 + A3");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
            VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("36")]
        public void SaveThenReadTest3()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "hello");
            Set(ss, "A2", "5.0");
            Set(ss, "A3", "4.0");
            Set(ss, "A4", "= A2 + A3");
            ss.Save("save6.txt");
            using (XmlReader reader = XmlReader.Create("save6.txt"))
            {
                int spreadsheetCount = 0;
                int cellCount = 0;
                bool A1 = false;
                bool A2 = false;
                bool A3 = false;
                bool A4 = false;
                string name = "UNDEFINED";
                string contents = "UNDEFINED";

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                Assert.AreEqual("default", reader["version"]);
                                spreadsheetCount++;
                                break;

                            case "cell":
                                cellCount++;
                                break;

                            case "name":
                                reader.Read();
                                name = reader.Value;
                                break;

                            case "contents":
                                reader.Read();
                                contents = reader.Value;
                                break;
                        }
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "cell":
                                if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
                                else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
                                else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
                                else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
                                else Assert.Fail();
                                break;
                        }
                    }
                }
                Assert.AreEqual(1, spreadsheetCount);
                Assert.AreEqual(4, cellCount);
                Assert.IsTrue(A1);
                Assert.IsTrue(A2);
                Assert.IsTrue(A3);
                Assert.IsTrue(A4);
            }
        }


        // Fun with formulas
        [TestMethod, Timeout(2000)]
        [TestCategory("37")]
        public void Formula1()
        {
            Formula1(new Spreadsheet());
        }
        public void Formula1(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= b1 + b2");
            Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
            Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
            Set(ss, "a3", "5.0");
            Set(ss, "b1", "2.0");
            Set(ss, "b2", "3.0");
            VV(ss, "a1", 10.0, "a2", 5.0);
            Set(ss, "b2", "4.0");
            VV(ss, "a1", 11.0, "a2", 6.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("38")]
        public void Formula2()
        {
            Formula2(new Spreadsheet());
        }
        public void Formula2(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= a3");
            Set(ss, "a3", "6.0");
            VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
            Set(ss, "a3", "5.0");
            VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("39")]
        public void Formula3()
        {
            Formula3(new Spreadsheet());
        }
        public void Formula3(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a3 + a5");
            Set(ss, "a2", "= a5 + a4");
            Set(ss, "a3", "= a5");
            Set(ss, "a4", "= a5");
            Set(ss, "a5", "9.0");
            VV(ss, "a1", 18.0);
            VV(ss, "a2", 18.0);
            Set(ss, "a5", "8.0");
            VV(ss, "a1", 16.0);
            VV(ss, "a2", 16.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("40")]
        public void Formula4()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formula1(ss);
            Formula2(ss);
            Formula3(ss);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("41")]
        public void Formula4a()
        {
            Formula4();
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("42")]
        public void MediumSheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
        }

        public void MediumSheet(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "1.0");
            Set(ss, "A2", "2.0");
            Set(ss, "A3", "3.0");
            Set(ss, "A4", "4.0");
            Set(ss, "B1", "= A1 + A2");
            Set(ss, "B2", "= A3 * A4");
            Set(ss, "C1", "= B1 + B2");
            VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
            Set(ss, "A1", "2.0");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
            Set(ss, "B1", "= A1 / A2");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("43")]
        public void MediumSheeta()
        {
            MediumSheet();
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("44")]
        public void MediumSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
            ss.Save("save7.txt");
            ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("45")]
        public void MediumSavea()
        {
            MediumSave();
        }


        // A long chained formula. Solutions that re-evaluate 
        // cells on every request, rather than after a cell changes,
        // will timeout on this test.
        // This test is repeated to increase its scoring weight
        [TestMethod, Timeout(6000)]
        [TestCategory("46")]
        public void LongFormulaTest()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(6000)]
        [TestCategory("47")]
        public void LongFormulaTest2()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(6000)]
        [TestCategory("48")]
        public void LongFormulaTest3()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(6000)]
        [TestCategory("49")]
        public void LongFormulaTest4()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(6000)]
        [TestCategory("50")]
        public void LongFormulaTest5()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        public void LongFormulaHelper(out object result)
        {
            try
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("sum1", "= a1 + a2");
                int i;
                int depth = 100;
                for (i = 1; i <= depth * 2; i += 2)
                {
                    s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                    s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
                }
                s.SetContentsOfCell("a" + i, "1");
                s.SetContentsOfCell("a" + (i + 1), "1");
                Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + i, "0");
                Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + (i + 1), "0");
                Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
                result = "ok";
            }
            catch (Exception e)
            {
                result = e;
            }
        }

    }
}

































//using SS;
//using SpreadsheetUtilities;
//using static SS.Spreadsheet;
//using System.Xml;
//using System.Xml.Linq;

///// <summary>
///// Author:   Adam Isaac
///// Partner:   None
///// Date:      Feb 9, 2024
///// Course:    CS 3500, University of Utah, School of Computing
///// Copyright: CS 3500 and [Adam Isaac - This work may not
/////            be copied for use in Academic Coursework.
/////
///// I, ADAM ISAAC, certify that I wrote this code from scratch and
///// did not copy it in part or whole from another source.  All
///// references used in the completion of the assignments are cited
///// in my README file.
/////
///// File Contents
///// This file is used for testing my spreadsheets capabilities. I use it to see if cells rely on one another or they go in circles
///// constantly trying to add values from other cells that dont exist. 
/////    
///// </summary>


//namespace SpreadsheetTests
//{


//    [TestClass]
//    public class SpreadsheetTests
//    {

//        private string _testDirectory = Path.Combine(Path.GetTempPath(), "SpreadsheetTests");

//        [TestInitialize]
//        public void Initialize()
//        {
//            // Ensure the test directory exists
//            Directory.CreateDirectory(_testDirectory);

//            File.WriteAllText(Path.Combine(_testDirectory, "validSpreadsheet.xml"), "<spreadsheet version=\"1.0\"></spreadsheet>");
//            // Create an invalid XML file (missing version attribute)
//            File.WriteAllText(Path.Combine(_testDirectory, "invalidSpreadsheet.xml"), "<spreadsheet></spreadsheet>");
//        }

//        [TestCleanup]
//        public void Cleanup()
//        {
//            // Clean up: Delete all files created during testing
//            foreach (var file in Directory.GetFiles(_testDirectory))
//            {
//                File.Delete(file);
//            }
//        }

















//        //[TestMethod]
//        //[ExpectedException(typeof(FormulaError))]
//        //public void GetCellValue_FormulaThrowsException_ReturnsFormulaError()
//        //{
//        //    // Arrange
//        //    var ss = new TestableSpreadsheet();
//        //    ss.SetContentsOfCell("A1", "=1/0"); // Setup that triggers an exception

//        //    // Act
//        //    var result = ss.GetCellValue("A1");

//        //    // Assert
//        //    // Check if result is of type FormulaError using pattern matching
//        //    // Assert
//        //    // Check if result is of type FormulaError using pattern matching

//        //}

//            [TestMethod]
//        public void SetCellContents_ReplacingFormulaWithDouble_AffectsDependentCells()
//        {
//            // Arrange
//            var ss = new TestableSpreadsheet();
//            ss.SetContentsOfCell("A1", "=B1 + C1"); // Initially, A1 depends on B1 and C1.
//            ss.SetContentsOfCell("B1", "1"); // Set initial values for B1 and C1 to ensure A1's formula can be calculated.
//            ss.SetContentsOfCell("C1", "1");

//            // Act
//            ss.SetContentsOfCell("A1", "5.0"); // Replace the formula in A1 with a double.

//            // Assert
//            // Verify that changing A1's content to a double doesn't throw an error and updates correctly.
//            // This indirectly checks if A1's dependencies on B1 and C1 are cleared because A1 no longer calculates based on them.
//            Assert.AreEqual(5.0, ss.GetCellValue("A1"), "A1 should now directly contain the value 5.0.");

//            // Additionally, you might want to check that changing B1 or C1 no longer affects A1.
//            ss.SetContentsOfCell("B1", "2");
//            Assert.AreEqual(5.0, ss.GetCellValue("A1"), "A1's value should remain unchanged after modifying B1, indicating dependencies are cleared.");
//        }

//        [TestMethod]
//        public void NewlyCreatedSpreadsheet_IsNotChanged()
//        {
//            var ss = new Spreadsheet();
//            Assert.IsFalse(ss.Changed, "A new spreadsheet should not be marked as changed.");
//        }


//        [TestMethod]
//        public void Spreadsheet_FourArgumentConstructor_InitializesCorrectly()
//        {
//            // Arrange
//            string pathToFile = "testPath.xml";
//            Func<string, bool> isValid = s => true;
//            Func<string, string> normalize = s => s.ToUpper();
//            string version = "1.0";

//            // Act
//            var ss = new Spreadsheet(pathToFile, isValid, normalize, version);

//            // Assert
//            // Verify that the Spreadsheet object is initialized with the correct properties
//            Assert.AreEqual(version, ss.Version);
//            // Assuming you store the path or have a related property to indicate readiness to load
//            // Assert.AreEqual(pathToFile, ss.PathToFile); 

//            // As file loading isn't implemented, other tests would be speculative
//            // Future tests should verify file loading and parsing once implemented
//        }

//        [TestMethod]
//        public void Spreadsheet_ThreeArgumentConstructor_InitializesCorrectly()
//        {
//            // Arrange
//            Func<string, bool> isValid = s => true; // Example validation function
//            Func<string, string> normalize = s => s.ToUpper(); // Example normalization function
//            string version = "1.0";

//            // Act
//            var ss = new Spreadsheet(isValid, normalize, version);

//            // Assert
//            // Verify that the Spreadsheet object is initialized with the provided version
//            // This assumes you have a way to retrieve the version, such as a Version property
//            Assert.AreEqual(version, ss.Version);

//            // Further tests could verify the behavior of isValid and normalize
//            // by interacting with the ss object, if such interactions are supported
//        }

//        [TestMethod]
//        public void ChangeInCellAffectsDirectAndIndirectDependents()
//        {
//            // Arrange
//            var ss = new Spreadsheet();
//            ss.SetContentsOfCell("A1", "2"); // Direct value
//            ss.SetContentsOfCell("B1", "=A1 * 2"); // Depends on A1
//            ss.SetContentsOfCell("C1", "=B1 + 3"); // Depends on B1

//            // Act

//            ss.SetContentsOfCell("A1", "3"); // Change that should affect B1 and C1

//            // Assert
//            // Verify that B1 and C1's values are as expected after the change in A1

//            Assert.AreEqual(6.0, ss.GetCellValue("B1")); // B1 should now be 3 * 2
//            Assert.AreEqual(9.0, ss.GetCellValue("C1")); // C1 should now be 6 + 3

//        }

//        [TestMethod]
//        public void GetCellValue_FormulaEvaluatesSuccessfully_ReturnsCorrectValue()
//        {
//            // Arrange
//            var ss = new TestableSpreadsheet();
//            ss.SetContentsOfCell("A1", "2"); // Set a cell value for reference in the formula
//            ss.SetContentsOfCell("B1", "=A1 * 2"); // Formula that should evaluate to 4

//            // Act
//            var result = ss.GetCellValue("B1");

//            // Assert
//            Assert.AreEqual(4.0, result); // Assuming Evaluate correctly computes the formula
//        }

//        [TestMethod]
//        public void GetCellValue_FormulaEvaluationFails_ReturnsFormulaError()
//        {
//            // Arrange
//            var ss = new TestableSpreadsheet();
//            ss.SetContentsOfCell("B1", "=1 / 0"); // Formula that should result in an error

//            // Act
//            object result = null;
//            try
//            {
//                result = ss.GetCellValue("B1");
//            }
//            catch (Exception ex)
//            {
//                // If any exception is thrown, fail the test because we expect a FormulaError return, not an exception
//                Assert.Fail("No exception should be thrown, but got: " + ex.GetType().FullName);
//            }

//            // Assert
//            Assert.IsNotNull(result, "Expected a non-null result when formula evaluation fails.");
//            Assert.IsInstanceOfType(result, typeof(SpreadsheetUtilities.FormulaError), "The result should be a FormulaError when formula evaluation fails.");
//        }

//        [TestMethod]
//        public void GetCellValue_CellIsEmpty_ReturnsEmptyString()
//        {
//            // Arrange
//            var ss = new TestableSpreadsheet();

//            // Act
//            var result = ss.GetCellValue("C1"); // Assuming C1 is not set and should be considered empty

//            // Assert
//            Assert.AreEqual("", result); // Verify that an empty string is returned for an uninitialized cell
//        }

//        [TestMethod]
//        public void GetXml_CellWithStringContent_ProducesCorrectXml()
//        {
//            // Arrange
//            var ss = new TestableSpreadsheet(); // Assuming TestableSpreadsheet exposes GetXML publicly
//            ss.SetContentsOfCell("A1", "Hello World");

//            // Act
//            string xmlOutput = ss.GetXML();
//            XElement xml = XElement.Parse(xmlOutput);

//            // Assert
//            var cell = xml.Element("cell");
//            Assert.IsNotNull(cell);
//            Assert.AreEqual("A1", cell.Element("name").Value);
//            Assert.AreEqual("Hello World", cell.Element("content").Value);
//            Assert.AreEqual("String", cell.Element("type").Value);
//        }

//        [TestMethod]
//        public void GetXml_CellWithFormulaContent_ProducesCorrectXml()
//        {
//            // Arrange
//            var ss = new TestableSpreadsheet(); // Assuming TestableSpreadsheet exposes GetXML publicly
//            ss.SetContentsOfCell("B1", "=A1+2");

//            // Act
//            string xmlOutput = ss.GetXML();
//            XElement xml = XElement.Parse(xmlOutput);

//            // Assert
//            var cell = xml.Element("cell");
//            Assert.IsNotNull(cell);
//            Assert.AreEqual("B1", cell.Element("name").Value);
//            Assert.AreEqual("=A1+2", cell.Element("content").Value);
//            Assert.AreEqual("Formula", cell.Element("type").Value);
//        }

//        [TestMethod]
//        [ExpectedException(typeof(SpreadsheetReadWriteException))]
//        public void GetSavedVersion_InvalidXmlContent_ThrowsSpreadsheetReadWriteException()
//        {
//            // Arrange
//            var ss = new Spreadsheet();
//            var filename = Path.Combine(_testDirectory, "InvalidXml.xml");

//            // Create an XML file with invalid content
//            string invalidXmlContent = "<spreadsheet><unclosedTag></spreadsheet>";
//            File.WriteAllText(filename, invalidXmlContent);

//            // Act
//            ss.GetSavedVersion(filename);

//            // The ExpectedException attribute should handle the assertion
//        }

//        [TestMethod]
//        [ExpectedException(typeof(SpreadsheetReadWriteException))]
//        public void GetSavedVersion_InvalidFile_ThrowsException()
//        {
//            var ss = new Spreadsheet();
//            string filename = Path.Combine(_testDirectory, "invalidSpreadsheet.xml");
//            ss.GetSavedVersion(filename);
//            // Expecting an exception due to missing version information
//        }

//        [TestMethod]
//        [ExpectedException(typeof(SpreadsheetReadWriteException))]
//        public void GetSavedVersion_NonexistentFile_ThrowsException()
//        {
//            var ss = new Spreadsheet();
//            string filename = Path.Combine(_testDirectory, "nonexistent.xml");
//            ss.GetSavedVersion(filename);
//            // Expecting an exception due to the file not existing
//        }

//        [TestMethod]
//        public void GetSavedVersion_ValidFile_ReturnsCorrectVersion()
//        {
//            var ss = new Spreadsheet();
//            string filename = Path.Combine(_testDirectory, "validSpreadsheet.xml");
//            string version = ss.GetSavedVersion(filename);
//            Assert.AreEqual("1.0", version, "The version should match the one specified in the file.");
//        }

//        [TestMethod]
//        public void Save_SpreadsheetWithVariousContents_WritesExpectedXml()
//        {
//            var ss = new TestableSpreadsheet();
//            string filename = Path.Combine(_testDirectory, "test.xml");
//            ss.SetContentsOfCell("A1", "Hello World");
//            ss.SetContentsOfCell("B1", "2.0");
//            ss.SetContentsOfCell("C1", "=A1+B1");

//            ss.Save(filename);

//            // Verify the file contents (simplified example)
//            Assert.IsTrue(File.Exists(filename), "The file should exist after saving.");

//            // Use XmlDocument or similar to parse and verify XML contents
//            XmlDocument doc = new XmlDocument();
//            doc.Load(filename);
//            // Add assertions to check the XML structure and contents
//        }


//        [TestMethod]
//        [ExpectedException(typeof(SpreadsheetReadWriteException))]
//        public void Save_InvalidPath_ThrowsSpreadsheetReadWriteException()
//        {
//            var ss = new TestableSpreadsheet();
//            string invalidFilename = Path.Combine(_testDirectory, "invalidPath//test.xml");

//            ss.Save(invalidFilename);
//            // Expected to throw due to invalid path
//        }



//        [TestMethod]
//        [ExpectedException(typeof(InvalidNameException))]
//        public void GetCellValue_InvalidName_ThrowsInvalidNameException()
//        {
//            var ss = new Spreadsheet();
//            ss.GetCellValue("InvalidName!");
//        }

//        [TestMethod]
//        public void GetCellValue_NonexistentCell_ReturnsEmptyString()
//        {
//            var ss = new Spreadsheet();
//            var result = ss.GetCellValue("A1");
//            Assert.AreEqual("", result);
//        }

//        [TestMethod]
//        public void GetCellValue_CellWithDouble_ReturnsDouble()
//        {
//            var ss = new TestableSpreadsheet();
//            double value = 2.5;
//            ss.TestSetCellContents("A1", value);
//            var result = ss.GetCellValue("A1");
//            Assert.AreEqual(2.5, result);
//        }

//        [TestMethod]
//        public void GetCellValue_CellWithString_ReturnsString()
//        {
//            var ss = new TestableSpreadsheet();
//            ss.TestSetCellContents("A1", "Test String");
//            var result = ss.GetCellValue("A1");
//            Assert.AreEqual("Test String", result);
//        }

//        [TestMethod]
//        [ExpectedException(typeof(InvalidNameException))]
//        public void SetContentsOfCell_InvalidName_ThrowsInvalidNameException()
//        {
//            var ss = new Spreadsheet();
//            ss.SetContentsOfCell("1Invalid", "10");
//        }

//        [TestMethod]
//        public void SetContentsOfCell_ValidDouble_ConvertsAndSetsCorrectly()
//        {
//            var ss = new Spreadsheet();
//            var affectedCells = ss.SetContentsOfCell("A1", "2.5");
//            Assert.IsTrue(affectedCells.Contains("A1"));
//            Assert.AreEqual(2.5, ss.GetCellValue("A1"));
//        }

//        [TestMethod]
//        [ExpectedException(typeof(FormulaFormatException))]
//        public void SetContentsOfCell_InvalidFormula_ThrowsException()
//        {
//            var ss = new Spreadsheet();
//            ss.SetContentsOfCell("A1", "=A2)");
//        }

//        [TestMethod]
//        public void GetXml_EmptySpreadsheet_ReturnsEmptyXml()
//        {
//            var ss = new Spreadsheet();
//            string expectedXml = "<spreadsheet></spreadsheet>";
//            Assert.AreEqual(expectedXml, ss.GetXML());
//        }

//        [TestMethod]
//        public void GetXml_SpreadsheetWithDouble_ReturnsCorrectXml()
//        {
//            var ss = new TestableSpreadsheet();
//            ss.TestSetCellContents("A1", 2.5);
//            string expectedXml = "<spreadsheet><cell><name>A1</name><content>2.5</content><type>String</type></cell></spreadsheet>";
//            Assert.AreEqual(expectedXml, ss.GetXML());
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void SetCellContents_NullFormula_ThrowsArgumentNullException()
//        {
//            var ss = new TestableSpreadsheet();
//          var result =  ss.TestSetCellContents(name: "A1", formula: null);
//        }


//        [TestMethod]
//        [ExpectedException(typeof(InvalidNameException))]
//        public void GetDirectDependents_ThrowsInvalidNameException_WhenNameIsInvalid()
//        {
//            // Arrange
//            var spreadsheet = new TestableSpreadsheet();
//            string invalidName = "1Invalid"; // Assuming this name is invalid based on your criteria.

//            // Act
//            var result = spreadsheet.TestGetDirectDependents(invalidName);

//            // Assert is handled by ExpectedException
//        }



//    [TestMethod]
//    [ExpectedException(typeof(ArgumentNullException))]
//    public void SetCellContents_ThrowsArgumentNullException_WhenFormulaIsNull()
//    {
//        // Arrange
//        var ss = new TestableSpreadsheet();

//        // Act
//        Formula nullFormula = null;

//        // Act
//        ss.TestSetCellContents("A1", nullFormula);

//        // Assert is handled by ExpectedException
//    }

//    [TestMethod]
//    public void EvaluateFormula_WithEmptyReferencedCell()
//    {
//        var ss = new TestableSpreadsheet();
//        ss.TestSetCellContents("A1", new Formula("B1 + 2"));

//        // Assuming a method exists to evaluate cell formulas and handle empty references gracefully
//        // var result = ss.EvaluateFormula("A1");
//        // Assert.AreEqual(2.0, result); // Assuming the default for empty cells in formula is 0
//    }

//    [TestMethod]
//    public void SetAndGetCellContents_ComplexFormula()
//    {
//        var ss = new TestableSpreadsheet();
//        ss.TestSetCellContents("A1", 2.0);
//        ss.TestSetCellContents("B1", 3.0);
//        var complexFormula = new Formula("A1 + B1 * 2");
//        ss.TestSetCellContents("C1", complexFormula);

//        // Assuming GetCellValue or a similar method exists to evaluate formulas
//        // var value = ss.GetCellValue("C1");
//        // Assert.AreEqual(8.0, value);
//    }
//    [TestMethod]
//    public void SetCellContent_WithExtremelyLargeNumber()
//    {
//        var ss = new TestableSpreadsheet();
//        double largeNumber = double.MaxValue;
//        ss.TestSetCellContents("B1", largeNumber);

//        Assert.AreEqual(largeNumber, ss.GetCellContents("B1"));
//    }

//    [TestMethod]
//    public void SetCellContent_ToEmptyString_ShouldClearCell()
//    {
//        var ss = new TestableSpreadsheet();
//        ss.TestSetCellContents("A1", "Non-empty");
//        ss.TestSetCellContents("A1", "");

//        Assert.AreEqual("", ss.GetCellContents("A1"));
//    }

//    [TestMethod]
//    public void SetCellContents_ValidNumber_ShouldSetCorrectly()
//    {
//        // Arrange
//        var spreadsheet = new TestableSpreadsheet();
//        string cellName = "A1";
//        double number = 5.0;

//        // Act
//        var affectedCells = spreadsheet.TestSetCellContents(cellName, number);

//        // Assert
//        Assert.IsTrue(affectedCells.Contains(cellName), "The affected cells should include the cell that was set.");
//        // Additional assertions can be made here to check if the cell's value is correctly set,
//        // depending on the implementation details of your Spreadsheet class.
//    }

//    [TestMethod]
//    [ExpectedException(typeof(InvalidNameException))]
//    public void SetCellContents_InvalidCellName_ShouldThrowInvalidNameException()
//    {
//        // Arrange
//        var spreadsheet = new TestableSpreadsheet();
//        string invalidCellName = null; // Assuming null is invalid. Adjust based on your validation logic.
//        double number = 10.0;

//        // Act
//        spreadsheet.TestSetCellContents(invalidCellName, number);

//        // No need for an Assert statement here due to ExpectedException attribute
//    }

//    [TestMethod]
//    public void SetAndGetCellContents_Number()
//    {
//        var ss = new TestableSpreadsheet();
//        ss.TestSetCellContents("A1", 5.0);

//        Assert.AreEqual(5.0, ss.GetCellContents("A1"));
//    }

//    [TestMethod]
//    public void SetAndGetCellContents_Text()
//    {
//        var ss = new TestableSpreadsheet();
//        ss.TestSetCellContents("A1", "Hello");

//        Assert.AreEqual("Hello", ss.GetCellContents("A1"));
//    }

//    [TestMethod]
//    public void SetAndGetCellContents_Formula()
//    {
//        var ss = new TestableSpreadsheet();
//        var formula = new Formula("B1 + C1");
//        ss.TestSetCellContents("A1", formula);

//        Assert.AreEqual(formula, ss.GetCellContents("A1"));
//    }

//    [TestMethod]
//    public void SetCellContents_ChangeTriggersDirectAndIndirectDependents_CorrectlyIdentifiesAllAffected()
//    {
//        // Arrange
//        var sheet = new TestableSpreadsheet();
//        sheet.TestSetCellContents("C1", 5.0); // Set initial contents
//        sheet.TestSetCellContents("B1", new Formula("C1 * 2")); // B1 directly depends on C1
//        sheet.TestSetCellContents("A1", new Formula("B1 * 2")); // A1 indirectly depends on C1 through B1

//        // Act
//        var affectedCells = sheet.TestSetCellContents("C1", 10.0); // Change C1's content

//        // Assert
//        Assert.IsTrue(affectedCells.Contains("C1"), "C1 should be affected.");
//        Assert.IsTrue(affectedCells.Contains("B1"), "B1 should be affected as a direct dependent.");
//        Assert.IsTrue(affectedCells.Contains("A1"), "A1 should be affected as an indirect dependent.");
//    }

//    [TestMethod]
//    [ExpectedException(typeof(InvalidNameException))]
//    public void GetCellContents_NullName_ThrowsInvalidNameException()
//    {
//        // Arrange
//        var spreadsheet = new Spreadsheet();
//        string cellName = null;

//        // Act
//        spreadsheet.GetCellContents(cellName);

//        // Assert is handled by ExpectedException
//    }

//    [TestMethod]
//    [ExpectedException(typeof(InvalidNameException))]
//    public void SetCellContents_ThrowsInvalidNameException_WhenNameIsInvalid()
//    {
//        var spreadsheet = new TestableSpreadsheet();
//        var formula = new Formula("2+2"); // Assuming a valid formula for the test
//        spreadsheet.TestSetCellContents("123InvalidName", formula); // This should throw an InvalidNameException
//    }


//    [TestMethod]
//    [ExpectedException(typeof(CircularException))]
//    public void SetCellContents_ThrowsCircularException_WhenCircularDependencyIsCreated()
//    {
//        var spreadsheet = new TestableSpreadsheet();
//        spreadsheet.TestSetCellContents("A1", new Formula("B1 + 1"));
//        spreadsheet.TestSetCellContents("B1", new Formula("C1 + 1")); // This should create a circular dependency and throw a CircularException
//        spreadsheet.TestSetCellContents("C1", new Formula("A1 + 1"));

//    }


//    [TestMethod]
//    [ExpectedException(typeof(CircularException))]
//    public void SetCellContents_ShouldThrowCircularException_WhenCircularDependencyDetected()
//    {
//        // Arrange
//        var spreadsheet = new TestableSpreadsheet(); // Replace with your actual class that contains CheckCycle
//        spreadsheet.TestSetCellContents("A1", new Formula("B1 + 1"));
//        spreadsheet.TestSetCellContents("B1", new Formula("C1 + 1"));
//        spreadsheet.TestSetCellContents("C1", new Formula("A1 + 1")); // This will create a circular dependency

//        // Act
//        // This line should trigger CheckCycle indirectly and throw a CircularException due to the circular reference
//        spreadsheet.TestSetCellContents("A1", new Formula("B1 * 2"));

//        // Assert is handled by ExpectedException
//    }


//    [TestMethod]
//    public void GetNamesOfAllNonemptyCells_ReturnsNonEmptyCellNames()
//    {
//        // Arrange
//        var spreadsheet = new TestableSpreadsheet();
//        spreadsheet.TestSetCellContents("A1", 5.0);
//        spreadsheet.TestSetCellContents("B1", "");
//        spreadsheet.TestSetCellContents("C1", "Hello");
//        spreadsheet.TestSetCellContents("D1", new Formula("A1 * 2"));

//        // Act
//        var nonEmptyCells = spreadsheet.GetNamesOfAllNonemptyCells().ToList();

//        // Assert
//        var expectedNonEmptyCells = new List<string> { "A1", "C1", "D1" }; // "B1" is empty and should not be included
//        CollectionAssert.AreEquivalent(expectedNonEmptyCells, nonEmptyCells, "The method should return the names of non-empty cells only.");
//    }

//    [TestMethod]
//    [ExpectedException(typeof(ArgumentNullException))]
//    public void SetCellContents_ThrowsArgumentNullException_WhenTextIsNull()
//    {
//        // Arrange
//        var spreadsheet = new TestableSpreadsheet();

//        // Act
//        spreadsheet.TestSetCellContents("A1", null as string);

//        // Assert is handled by ExpectedException
//    }

//    [TestMethod]
//    [ExpectedException(typeof(InvalidNameException))]
//    public void SetCellContents_ThrowsInvalidNameException_WhenNameIsNull()
//    {
//        // Arrange
//        var spreadsheet = new TestableSpreadsheet();

//        // Act
//        spreadsheet.TestSetCellContents(null, "text");

//        // Assert is handled by ExpectedException
//    }

//    [TestMethod]
//    public void SetCellContents_UpdatesCell_WhenCellAlreadyHasContent()
//    {
//        // Arrange
//        var spreadsheet = new TestableSpreadsheet();
//        spreadsheet.TestSetCellContents("A1", "Initial Content");

//        // Act
//        spreadsheet.TestSetCellContents("A1", "Updated Content");
//        var content = spreadsheet.GetCellContents("A1");

//        // Assert
//        Assert.AreEqual("Updated Content", content, "Cell content should be updated to the new text.");
//    }
//    [TestMethod]
//    public void SetCellContents_RemovesCell_WhenTextIsEmpty()
//    {
//        // Arrange
//        var spreadsheet = new TestableSpreadsheet();
//        spreadsheet.TestSetCellContents("A1", "Initial Content");

//        // Act
//        spreadsheet.TestSetCellContents("A1", "");
//        var nonEmptyCells = spreadsheet.GetNamesOfAllNonemptyCells();

//        // Assert
//        Assert.IsFalse(nonEmptyCells.Contains("A1"), "Cell should be removed when set with empty text.");
//    }

//    [TestMethod]
//    [ExpectedException(typeof(ArgumentNullException))]
//    public void GetDirectDependents_ThrowsArgumentNullException_WhenNameIsNull()
//    {
//        // Arrange
//        var spreadsheet = new TestableSpreadsheet();

//        // Act - Attempt to call the exposed GetDirectDependents with null
//        var result = spreadsheet.TestGetDirectDependents(null);

//        // Assert is handled by ExpectedException attribute
//    }

//    [TestMethod]
//    public void ChangeCellContent_UpdatesDirectDependent()
//    {
//        var ss = new TestableSpreadsheet();
//        ss.TestSetCellContents("B1", 2.0);
//        ss.TestSetCellContents("A1", new Formula("B1 * 2"));

//        ss.TestSetCellContents("B1", 3.0); // Change B1, which A1 depends on
//        var affectedCells = ss.TestSetCellContents("B1", 3.0);

//        Assert.IsTrue(affectedCells.Contains("A1"), "A1 should be affected as it directly depends on B1.");
//    }

//    [TestMethod]
//    public void ChangeCellContent_UpdatesIndirectDependent()
//    {
//        var ss = new TestableSpreadsheet();
//        ss.TestSetCellContents("C1", 2.0);
//        ss.TestSetCellContents("B1", new Formula("C1 * 2"));
//        ss.TestSetCellContents("A1", new Formula("B1 + 2"));

//        ss.TestSetCellContents("C1", 3.0); // Change C1, which B1 depends on, and thus A1 indirectly
//        var affectedCells = ss.TestSetCellContents("C1", 3.0);

//        Assert.IsTrue(affectedCells.Contains("A1"), "A1 should be affected as it indirectly depends on C1 through B1.");
//    }

//        [TestMethod]
//        [ExpectedException(typeof(InvalidNameException))]
//        public void GetCellValue_InvalidName_ThrowsInvalidNameException2()
//        {
//            // Arrange
//            var ss = new TestableSpreadsheet();
//            string invalidName = "123Invalid"; // Example of an invalid name, adjust based on your validation logic

//            // Act
//            ss.GetCellValue(invalidName);

//            // Assert is handled by the ExpectedException attribute
//        }

//    }



//}