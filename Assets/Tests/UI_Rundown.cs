using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System;

namespace Tests
{
    public class UI_Rundown
    {
        [UnityTest]
        public IEnumerator Bootloader() //WithEnumeratorPasses
        {
            SceneManager.LoadScene("Bootloader");
            yield return new WaitForSeconds(1);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Bootloader");

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }

        [UnityTest]
        public IEnumerator NavigationNotLoggedIn() //WithEnumeratorPasses
        {
            SceneManager.LoadScene("Bootloader");
            yield return new WaitForSeconds(1);

            SceneManager.LoadScene("Login");
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Login");

            GameObject.Find("Login With Email").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "LoginWithEmail");

            GameObject.Find("Back Button").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Login");

            GameObject.Find("Sign Up").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "SignUp");

            GameObject.Find("Go To Login").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Login");

            yield return null;
        }

        [UnityTest]
        public IEnumerator SignInThenOut() //WithEnumeratorPasses
        {
            LogAssert.ignoreFailingMessages = true;

            SceneManager.LoadScene("Bootloader");
            yield return new WaitForSeconds(1);

            SceneManager.LoadScene("Login");
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Login");

            GameObject.Find("Login With Email").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "LoginWithEmail");

            GameObject.Find("Email Input Field").GetComponent<InputField>().text = "demo2@gmail.com";
            GameObject.Find("Password Input Field").GetComponent<InputField>().text = "12345678";
            GameObject.Find("Login Button").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(2);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "MainMenu");

            yield return new WaitForSeconds(1);
            GameObject.Find("Log Out Button").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(1);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Login");

            yield return null;
        }

        [UnityTest]
        public IEnumerator SignInThenOut_NewUser() //WithEnumeratorPasses
        {
            var userName = "";
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var stringChars = new char[6];
            var random = new System.Random();

            for (int i = 0; i < stringChars.Length; i++) {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            userName = new String(stringChars) + "@gmail.com";

            LogAssert.ignoreFailingMessages = true;

            SceneManager.LoadScene("Bootloader");
            yield return new WaitForSeconds(1);

            SceneManager.LoadScene("Login");
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Login");

            GameObject.Find("Sign Up").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "SignUp");

            // Make new user
            GameObject.Find("Email Input Field").GetComponent<InputField>().text = userName;
            GameObject.Find("Password Input Field").GetComponent<InputField>().text = "12345678";
            GameObject.Find("Submit").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(3);

            GameObject.Find("Go To Login").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Login");

            GameObject.Find("Login With Email").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(2f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "LoginWithEmail");

            // Sign in as new user
            GameObject.Find("Email Input Field").GetComponent<InputField>().text = userName;
            GameObject.Find("Password Input Field").GetComponent<InputField>().text = "12345678";
            GameObject.Find("Login Button").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(1);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "MainMenu");

            yield return new WaitForSeconds(1);
            GameObject.Find("Log Out Button").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(1);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Login");

            yield return null;
        }

        [UnityTest]
        public IEnumerator EnterMatch() //WithEnumeratorPasses
        {
            LogAssert.ignoreFailingMessages = true;

            SceneManager.LoadScene("Bootloader");
            yield return new WaitForSeconds(1);

            SceneManager.LoadScene("Login");
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Login");

            GameObject.Find("Login With Email").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "LoginWithEmail");

            GameObject.Find("Email Input Field").GetComponent<InputField>().text = "demo2@gmail.com";
            GameObject.Find("Password Input Field").GetComponent<InputField>().text = "12345678";
            GameObject.Find("Login Button").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(2);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "MainMenu");

            /*
            var buttArr = Resources.FindObjectsOfTypeAll(typeof(Button));
            foreach (var button in buttArr)
            {
                Debug.Log("Buttons: " + button);
            }
            */

            try
            {
                GameObject.Find("GameButton").GetComponent<Button>().onClick.Invoke();
            }
            catch
            {
                try
                {
                    GameObject.Find("GameButton(Clone)").GetComponent<Button>().onClick.Invoke();
                }
                catch { }
            }

            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Match"); // Weirdo
            yield return new WaitForSeconds(1);

            GameObject.Find("Back").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(2);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "MainMenu");


            // Log out
            yield return new WaitForSeconds(1);
            GameObject.Find("Log Out Button").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(1);
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Login");

            yield return null;
        }
    }
}
