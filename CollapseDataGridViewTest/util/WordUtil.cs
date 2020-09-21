using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpWord = Microsoft.Office.Interop.Word;

namespace CollapseDataGridViewTest.util
{
    public class WordUtil
    {

        public static List<string> CollectTagsByConventionalWildCard(string path)
        {
            List<string> tags = new List<string>();
            var doc = GetDocument(path);
            doc.Item1.Selection.Find.ClearFormatting();

            Microsoft.Office.Interop.Word.Range rng = doc.Item1.Selection.Range;
            rng.Find.MatchWildcards = true;
            rng.Find.Text = "[{]*[}]";

            while (rng.Find.Execute())
            {
                object cstart = rng.Start;
                object cend = rng.End;
                Microsoft.Office.Interop.Word.Range localrng = doc.Item2.Range(ref cstart, ref cend);
                tags.Add(localrng.Text);
            }
            ReleaseAll(doc);
            return tags;
        }

        public static Tuple<OpWord.Application, OpWord.Document> GetDocument(string file)
        {

            var app = new OpWord.Application();
            app.Visible = false;

            var document = app.Documents.Open(
                file, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            var result = new Tuple<OpWord.Application, OpWord.Document>(app, document);
            return result;
        }

        public static void ReleaseAll(Tuple<OpWord.Application, OpWord.Document> ad)
        {
            ad.Item2.Close(false, Missing.Value, Missing.Value);
            NAR(ad.Item2);
            ad.Item1.Quit();
            NAR(ad.Item1);
            System.GC.Collect();
        }

        public static void ReplaceText(Tuple<OpWord.Application, OpWord.Document> doc, string strTag, string strNew)
        {
            if (string.IsNullOrEmpty(strNew))
                return;

            object nullobj = Type.Missing, missing = Type.Missing;
            object objReplace = OpWord.WdReplace.wdReplaceAll;

            doc.Item1.Selection.Find.ClearFormatting();
            doc.Item1.Selection.Find.Replacement.ClearFormatting();

            foreach (Microsoft.Office.Interop.Word.Section section in doc.Item2.Sections)
            {
                Microsoft.Office.Interop.Word.Range fphFooterRange = section.Footers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterFirstPage].Range;
                fphFooterRange.Find.Text = strTag;
                fphFooterRange.Find.Replacement.Text = strNew;
                fphFooterRange.Find.Execute(ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref objReplace, ref missing, ref missing, ref missing, ref missing);

                Microsoft.Office.Interop.Word.Range fpHeaderRange = section.Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterFirstPage].Range;
                fpHeaderRange.Find.Text = strTag;
                fpHeaderRange.Find.Replacement.Text = strNew;
                fpHeaderRange.Find.Execute(ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref objReplace, ref missing, ref missing, ref missing, ref missing);

                //Get the header range and add the header details.
                Microsoft.Office.Interop.Word.Range headerRange = section.Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                Microsoft.Office.Interop.Word.Range footerRange = section.Footers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                footerRange.Find.Text = strTag;
                footerRange.Find.Replacement.Text = strNew;
                footerRange.Find.Execute(ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref objReplace, ref missing, ref missing, ref missing, ref missing);


                headerRange.Find.Text = strTag;
                headerRange.Find.Replacement.Text = strNew;
                headerRange.Find.Execute(ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref objReplace, ref missing, ref missing, ref missing, ref missing);
            }

            if (strNew.Length <= 255)
            {
                doc.Item1.Selection.Find.Text = strTag;
                doc.Item1.Selection.Find.Replacement.Text = strNew;

                doc.Item1.Selection.Find.Execute(ref nullobj, ref nullobj, ref nullobj,
                                           ref nullobj, ref nullobj, ref nullobj,
                                           ref nullobj, ref nullobj, ref nullobj,
                                           ref nullobj, ref objReplace, ref nullobj,
                                           ref nullobj, ref nullobj, ref nullobj);
            }
            else
            {
                int hasSubLength = 0;
                int itemLength = 255 - strTag.Length;
                while (true)
                {
                    string itemStr = "";
                    int subLength = 0;
                    if (strNew.Length - hasSubLength <= 255)  // 剩余的内容不超过255，直接替换
                    {
                        doc.Item1.Selection.Find.ClearFormatting();
                        doc.Item1.Selection.Find.Replacement.ClearFormatting();
                        doc.Item1.Selection.Find.Text = strTag;
                        doc.Item1.Selection.Find.Replacement.Text = strNew.Substring(hasSubLength, strNew.Length - hasSubLength);
                        doc.Item1.Selection.Find.Execute(ref nullobj, ref nullobj, ref nullobj,
                                                    ref nullobj, ref nullobj, ref nullobj,
                                                    ref nullobj, ref nullobj, ref nullobj,
                                                    ref nullobj, ref objReplace, ref nullobj,
                                                    ref nullobj, ref nullobj, ref nullobj);

                        break; // 结束循环
                    }

                    // 由于Word中换行为“^p”两个字符不能分割
                    // 如果分割位置将换行符分开了，则本次少替换一个字符
                    if (strNew.Substring(hasSubLength, itemLength).EndsWith("^") &&
                        strNew.Substring(hasSubLength, itemLength + 1).EndsWith("p"))
                    {
                        subLength = itemLength - 1;
                    }
                    else
                    {
                        subLength = itemLength;
                    }
                    itemStr = strNew.Substring(hasSubLength, subLength) + strTag;
                    doc.Item1.Selection.Find.ClearFormatting();
                    doc.Item1.Selection.Find.Replacement.ClearFormatting();
                    doc.Item1.Selection.Find.Text = strTag;
                    doc.Item1.Selection.Find.Replacement.Text = itemStr;
                    doc.Item1.Selection.Find.Execute(ref nullobj, ref nullobj, ref nullobj,
                                                ref nullobj, ref nullobj, ref nullobj,
                                                ref nullobj, ref nullobj, ref nullobj,
                                                ref nullobj, ref objReplace, ref nullobj,
                                                ref nullobj, ref nullobj, ref nullobj);
                    hasSubLength += subLength;
                }
            }
            //  doc.Item2.Save();
            // ReleaseAll(doc);
        }

        public static void ReplaceImage(Tuple<OpWord.Application, OpWord.Document> doc, string strTag, string strPictureFileName)
        {
            object nullobj = Type.Missing;

            object objReplace = OpWord.WdReplace.wdReplaceAll;

            OpWord.Selection selection = doc.Item1.Selection;

            object unite = OpWord.WdUnits.wdStory;

            selection.Find.Text = strTag;

            //  selection.Find.Replacement.Text = "";
            selection.Find.Execute();
            //selection.Find.Execute(ref nullobj, ref nullobj, ref nullobj,
            //                          ref nullobj, ref nullobj, ref nullobj,
            //                          ref nullobj, ref nullobj, ref nullobj,
            //                          ref nullobj, ref objReplace, ref nullobj,
            //                          ref nullobj, ref nullobj, ref nullobj);

            //  selection.EndKey(ref unite, ref nullobj);

            //定义该插入的图片是否为外部链接
            Object linkToFile = false;               //默认,这里貌似设置为bool类型更清晰一些
            //定义要插入的图片是否随Word文档一起保存
            Object saveWithDocument = true;              //默认

            Object range = selection.Range;

            OpWord.InlineShape inlineShape = doc.Item2.InlineShapes.AddPicture(strPictureFileName, ref linkToFile, ref saveWithDocument, ref range);

            //设置图片大小
            inlineShape.Width = 100;
            inlineShape.Height = 100;

            selection.Find.Text = strTag;
            selection.Find.Replacement.Text = "";
            selection.Find.Execute(ref nullobj, ref nullobj, ref nullobj,
                                      ref nullobj, ref nullobj, ref nullobj,
                                      ref nullobj, ref nullobj, ref nullobj,
                                      ref nullobj, ref objReplace, ref nullobj,
                                      ref nullobj, ref nullobj, ref nullobj);

            //  doc.Item2.Save();
        }

        [STAThread]
        public static void ReplaceImage(Tuple<OpWord.Application, OpWord.Document> doc, string strTag, Image image)
        {
            ReplaceImage(doc, strTag, image, 100, 100);
        }

        [STAThread]
        public static void ReplaceImage(Tuple<OpWord.Application, OpWord.Document> doc, string strTag, Image image, int width, int height)
        {
            object nullobj = Type.Missing;

            object objReplace = OpWord.WdReplace.wdReplaceAll;

            OpWord.Selection selection = doc.Item1.Selection;

            object unite = OpWord.WdUnits.wdStory;

            selection.Find.Text = strTag;

            var isSelect = selection.Find.Execute();
            if (!isSelect)
            {
                return;
            }

            var range = selection.Range;

            System.Windows.Forms.Clipboard.SetDataObject(image);
            range.Paste();

            var inlineShape = range.InlineShapes[1];
            //设置图片大小
            inlineShape.Width = width;
            inlineShape.Height = height;

            selection.Find.Text = strTag;
            selection.Find.Replacement.Text = "";
            selection.Find.Execute(ref nullobj, ref nullobj, ref nullobj,
                                      ref nullobj, ref nullobj, ref nullobj,
                                      ref nullobj, ref nullobj, ref nullobj,
                                      ref nullobj, ref objReplace, ref nullobj,
                                      ref nullobj, ref nullobj, ref nullobj);

            //  doc.Item2.Save();
            System.Windows.Forms.Clipboard.Clear();
        }

        public static void SaveDoc(OpWord.Document doc)
        {
            doc.Save();
        }

        public void test()
        {
            var ad = WordUtil.GetDocument(@"C:\temp\test.docx");
            try
            {
                // ReplaceText(ad,"123", "11111111111111112222222222222222222222222222222222222222222222222222222222222222111111111111111111111111111111111111111111222222222222222222222222222222222222222222222211111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111122222222222222222222233333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333331111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111");
                //TODO cook word file
                ReplaceImage(ad, "123", @"C:\temp\123.bmp");
            }
            finally
            {
                ReleaseAll(ad);
            }
        }

        private static void NAR(object o)
        {
            try { System.Runtime.InteropServices.Marshal.ReleaseComObject(o); } catch { } finally { o = null; }
        }
    }
}
