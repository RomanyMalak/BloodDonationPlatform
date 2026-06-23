import { Component, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

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
    this.simulateBotReply(text);
  }

  sendMessage() {
    if (!this.userMessage.trim()) return;
    this.addMessage('user', this.userMessage);
    this.simulateBotReply(this.userMessage);
    this.userMessage = '';
  }

  addMessage(from: 'bot' | 'user', text: string) {
    this.messages.push({ from, text });
  }

  simulateBotReply(userText: string) {
    this.isTyping = true;
    setTimeout(() => {
      this.isTyping = false;
      const reply = this.getBotReply(userText);
      this.addMessage('bot', reply);
    }, 1500);
  }

  getBotReply(text: string): string {
    if (text.includes('تبرع')) return 'رائع! يمكنك التبرع بالدم من خلال الضغط على "طلبات التبرع" واختيار أقرب مستشفى لك.';
    if (text.includes('عاجل')) return 'حالة عاجلة؟ اضغط على "إنشاء طلب جديد" وسيقوم نظامنا بإيجاد أقرب متبرع فوراً!';
    if (text.includes('بنك دم')) return 'يمكنك البحث عن أقرب بنك دم من خلال قسم "المراكز الصحية" في القائمة الرئيسية.';
    if (text.includes('دعم')) return 'يمكنك التواصل مع فريق الدعم على البريد: support@bloodhub.sa أو الاتصال على 920XXXXXX.';
    return 'شكراً على تواصلك! كيف يمكنني مساعدتك بشكل أفضل؟';
  }

  ngAfterViewChecked() {
    if (this.messagesContainer) {
      const el = this.messagesContainer.nativeElement;
      el.scrollTop = el.scrollHeight;
    }
  }
}