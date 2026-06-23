import { Component } from '@angular/core';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar';

@Component({
  standalone: true,
  selector: 'app-ai-pipeline',
  imports: [SidebarComponent],
  templateUrl: './ai-pipeline.html',
  styleUrls: ['./ai-pipeline.css'],
})
export class AiPipeline {}
