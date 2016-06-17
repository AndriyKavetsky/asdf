using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace git.Models
{
    public class Steganography
    {
        public enum State
        {
            Hiding,
            Filling_With_Zeros
        };

        public static Bitmap embedText(string text, Bitmap bmp)
        {
            // шифрування символів у зображенні initially, we'll be hiding characters in the image
            State state = State.Hiding;

            // індекс символа який ми ховаємо holds the index of the character that is being hidden
            int charIndex = 0;

            // значення символа переведеного в цілочисельний тип holds the value of the character converted to integer
            int charValue = 0;

            // індекс кольору поточного елемента holds the index of the color element (R or G or B) that is currently being processed
            long pixelElementIndex = 0;

            // кількість нудів які були додані після додавання символів holds the number of trailing zeros that have been added when finishing the process
            int zeros = 0;

            // елементи пікселя hold pixel elements
            int R = 0, G = 0, B = 0;

            // проходимось по рядках pass through the rows
            for (int i = 0; i < bmp.Height; i++)
            {
                // проходимось по стовпцях pass through each row
                for (int j = 0; j < bmp.Width; j++)
                {
                    //  поточний піксель який обробляється holds the pixel that is currently being processed
                    Color pixel = bmp.GetPixel(j, i);

                    // видаляємо найменш значущий біт now, clear the least significant bit (LSB) from each pixel element
                    R = pixel.R - pixel.R % 2;
                    G = pixel.G - pixel.G % 2;
                    B = pixel.B - pixel.B % 2;
                    //MessageBox.Show(pixel.B+"  %2   = "+pixel.B%2);
                    // проходимось по змінних кожного пікселя for each pixel, pass through its elements (RGB)
                    for (int n = 0; n < 3; n++)
                    {
                        // перевіряємо чи нові 8 біт біли обробленими check if new 8 bits has been processed
                        if (pixelElementIndex % 8 == 0)
                        {
                            // перевіряємо чи процес завершився check if the whole process has finished
                            // процес завершується якщо додається 8 нулів we can say that it's finished when 8 zeros are added
                            if (state == State.Filling_With_Zeros && zeros == 8)
                            {
                                // обробляємо останній піксель в зображені apply the last pixel on the image
                                // навіть якщо частина його елементів була змінена even if only a part of its elements have been affected
                                if ((pixelElementIndex - 1) % 3 < 2)
                                {
                                    bmp.SetPixel(j, i, Color.FromArgb(R, G, B));
                                }

                                // повернаємо зображення із внесеним в нього текстом return the bitmap with the text hidden in
                                return bmp;
                            }

                            // перевіряємо чи всі символи були схованими в зображенні check if all characters has been hidden
                            if (charIndex >= text.Length)
                            {
                                // починаємо додавати нулі щоб позначити кінець тексту start adding zeros to mark the end of the text
                                state = State.Filling_With_Zeros;
                            }
                            else
                            {
                                // інакше посуваємось на один символ вперед move to the next character and process again
                                charValue = text[charIndex++];
                                //??MessageBox.Show("charValue   " + charValue + "    text[charIndex++] " + text[charIndex++]);
                            }
                        }

                        // перевіряємо в який піксель пторібно внести зміни в наменш значущому біті check which pixel element has the turn to hide a bit in its LSB
                        switch (pixelElementIndex % 3)
                        {
                            case 0:
                                {
                                    if (state == State.Hiding)
                                    {
                                        // крайній правий біт символа буде (charValue % 2) the rightmost bit in the character will be (charValue % 2)
                                        // перемістимо це занчення замість lsb в значенні пікселя to put this value instead of the LSB of the pixel element
                                        // додаємо це значення до нього just add it to it
                                        // перед цим lsb елемент в пікселі був очищеним  перед цією операцією recall that the LSB of the pixel element had been cleared
                                        // before this operation
                                        R += charValue % 2;

                                        // видаляємо крайній правий біт символу removes the added rightmost bit of the character
                                        // так щоб можна було досягнути наступний біт such that next time we can reach the next one
                                        charValue /= 2;
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if (state == State.Hiding)
                                    {
                                        G += charValue % 2;

                                        charValue /= 2;
                                    }
                                }
                                break;
                            case 2:
                                {
                                    if (state == State.Hiding)
                                    {
                                        B += charValue % 2;

                                        charValue /= 2;
                                    }

                                    bmp.SetPixel(j, i, Color.FromArgb(R, G, B));
                                }
                                break;
                        }

                        pixelElementIndex++;

                        if (state == State.Filling_With_Zeros)
                        {
                            // збільшуємо значення нулів поки не буде 8 нулів increment the value of zeros until it is 8
                            zeros++;
                        }
                    }
                }
            }

            return bmp;
        }

        public static string extractText(Bitmap bmp)
        {
            int colorUnitIndex = 0;
            int charValue = 0;

            // holds the text that will be extracted from the image
            string extractedText = String.Empty;

            // pass through the rows
            for (int i = 0; i < bmp.Height; i++)
            {
                // pass through each row
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color pixel = bmp.GetPixel(j, i);

                    // for each pixel, pass through its elements (RGB)
                    for (int n = 0; n < 3; n++)
                    {
                        switch (colorUnitIndex % 3)
                        {
                            case 0:
                                {
                                    // get the LSB from the pixel element (will be pixel.R % 2)
                                    // then add one bit to the right of the current character
                                    // this can be done by (charValue = charValue * 2)
                                    // replace the added bit (which value is by default 0) with
                                    // the LSB of the pixel element, simply by addition
                                    charValue = charValue * 2 + pixel.R % 2;
                                }
                                break;
                            case 1:
                                {
                                    charValue = charValue * 2 + pixel.G % 2;
                                }
                                break;
                            case 2:
                                {
                                    charValue = charValue * 2 + pixel.B % 2;
                                }
                                break;
                        }

                        colorUnitIndex++;

                        // if 8 bits has been added, then add the current character to the result text
                        if (colorUnitIndex % 8 == 0)
                        {
                            // reverse? of course, since each time the process happens on the right (for simplicity)
                            charValue = reverseBits(charValue);

                            // can only be 0 if it is the stop character (the 8 zeros)
                            if (charValue == 0)
                            {
                                return extractedText;
                            }

                            // convert the character value from int to char
                            char c = (char)charValue;

                            // add the current character to the result text
                            extractedText += c.ToString();
                        }
                    }
                }
            }

            return extractedText;
        }

        public static int reverseBits(int n)
        {
            int result = 0;

            for (int i = 0; i < 8; i++)
            {
                result = result * 2 + n % 2;

                n /= 2;
            }

            return result;
        }
    }
}
