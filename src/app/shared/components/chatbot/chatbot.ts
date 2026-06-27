import { Component, ViewChild, ElementRef, AfterViewChecked, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatbotService } from '../../../core/services/chatbot.service'; 

interface Message {
  from: 'bot' | 'user';
  text: string;
}

@Component({
  selector: 'app-chatbot',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chatbot.html',
  styleUrl: './chatbot.css'
})
export class Chatbot implements AfterViewChecked {

  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  private chatbotService = inject(ChatbotService);

  isOpen = false;
  showTooltip = false;
  isTyping = false;
  userMessage = '';
  messages: Message[] = [];

  toggleChat() {
    this.isOpen = !this.isOpen;
    this.showTooltip = false;
  }

  sendQuick(text: string) {
    this.addMessage('user', text);
    this.callBotApi(text);
  }

  sendMessage() {
    if (!this.userMessage.trim()) return;
    const msg = this.userMessage;
    this.addMessage('user', msg);
    this.userMessage = '';
    this.callBotApi(msg);
  }

  addMessage(from: 'bot' | 'user', text: string) {
    this.messages.push({ from, text });
  }

  

  callBotApi(userText: string) {
    this.isTyping = true;

    this.chatbotService.sendMessage(userText).subscribe({
      next: (res) => {
        this.isTyping = false;
        if (res.success) {
          this.addMessage('bot', res.answer);
        } else {
          this.addMessage('bot', 'حدث خطأ، حاولي مرة أخرى.');
        }
      },
      error: () => {
        this.isTyping = false;
        this.addMessage('bot', 'تعذر الاتصال بالخادم، حاولي لاحقاً.');
      }
    });
  }

  ngAfterViewChecked() {
    if (this.messagesContainer) {
      const el = this.messagesContainer.nativeElement;
      el.scrollTop = el.scrollHeight;
    }
  }
}