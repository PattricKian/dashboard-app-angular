import { Component } from '@angular/core';
import { DashboardControl, DashboardControlArgs } from 'devexpress-dashboard';
import { DataRequestOptions, ItemDataLoadingMode } from 'devexpress-dashboard';

import { TextBoxItemEditorExtension } from 'devexpress-dashboard/designer/text-box-item-editor-extension';
import { SimpleTableItemExtension } from './extensions/simple-table-item';
import { WebPageItemExtension } from './extensions/webpage-item';
import { ParameterItemExtension } from './extensions/parameter-item';
import { FunnelD3ItemExtension } from './extensions/funnel-d3-item';
import { DashboardDescriptionExtension } from './extensions/dashboard-description-extension';
import { ItemDescriptionExtension } from './extensions/item-description-extension';
import { ChartLineOptionsExtension } from './extensions/chart-line-options-extension';
import { GanttItemExtension } from './extensions/gantt-item';
import { OnlineMapItemExtension } from './extensions/online-map-item';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  workingMode: string = 'Viewer';
  dashboardId: string = 'support';
  dashboards = [
    { id: 'support', name: 'Support' },
    { id: 'products', name: 'SA Test' },
  ];
  get workingModeText() {
    return 'Switch to ' + this.toggleMode(this.workingMode);
  }
  changeWorkingMode() {
    this.workingMode = this.toggleMode(this.workingMode);
  }
  toggleMode(mode) {
    return mode === 'Viewer' ? 'Designer' : 'Viewer';
  }
  onBeforeRender(args: DashboardControlArgs) {
    var dashboardControl = args.component;

    dashboardControl.registerExtension(
      new ChartLineOptionsExtension(dashboardControl)
    );
    dashboardControl.registerExtension(
      new TextBoxItemEditorExtension(dashboardControl)
    );
    dashboardControl.registerExtension(
      new SimpleTableItemExtension(dashboardControl)
    );
    dashboardControl.registerExtension(
      new WebPageItemExtension(dashboardControl)
    );
    dashboardControl.registerExtension(
      new ParameterItemExtension(dashboardControl)
    );
    dashboardControl.registerExtension(
      new FunnelD3ItemExtension(dashboardControl)
    );
    dashboardControl.registerExtension(
      new DashboardDescriptionExtension(dashboardControl)
    );
    dashboardControl.registerExtension(
      new ItemDescriptionExtension(dashboardControl)
    );
    dashboardControl.registerExtension(
      new GanttItemExtension(dashboardControl)
    );
    dashboardControl.registerExtension(
      new OnlineMapItemExtension(dashboardControl)
    );
  }
}
