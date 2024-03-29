﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using SignalRClientWPF.Dto;

namespace SignalRClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public System.Threading.Thread Thread { get; set; }
        public string Host = "https://localhost:44361/";

        public IHubProxy Proxy { get; set; }
        public HubConnection Connection { get; set; }

        public bool Active { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }


 
        private async void ActionSendButtonClick(object sender, RoutedEventArgs e)
        {
            await SendMessage();
        }

   
        private async Task SendMessage()
        {
            await Proxy.Invoke("SendMessage", lblConid.Content, MessageTextBox.Text);
         
        }

 
   
        private async void ActionWindowLoaded(object sender, RoutedEventArgs e)
        {
            Active = true;
            Thread = new System.Threading.Thread(() =>
            {
                Connection = new HubConnection(Host);
                Proxy = Connection.CreateHubProxy("QNZChatHub");

                Proxy.On<string, string>("addMessage", (message, connectionId) => OnSendData(message, connectionId));
               
                Connection.Start().Wait(); 

                Proxy.Invoke("GetName", "客服").Wait();

                while (Active)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }) { IsBackground = true };
            Thread.Start();

           
        }

        private void OnSendData(string message, string connectionId)
        {
           // lblConid.Content = connectionId;
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() => lblConid.Content = connectionId));
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() => MessagesListBox.Items.Insert(0, message)));
        }

        private async void ActionMessageTextBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                await SendMessage();
                MessageTextBox.Text = "";
            }
        }

        //private void LoginIn_Click(object sender, RoutedEventArgs e)
        //{
        //    Active = true;
        //    Thread = new System.Threading.Thread(() =>
        //    {
        //        Connection = new HubConnection(Host);
        //        Proxy = Connection.CreateHubProxy("QNZChatHub");

        //        Proxy.On<string, string>("addMessage", (message, connectionId) => OnSendData(message, connectionId));

        //        Connection.Start();

        //        while (Active)
        //        {
        //            System.Threading.Thread.Sleep(10);
        //        }
        //    })
        //    { IsBackground = true };
        //    Thread.Start();
        //}
    }
}
