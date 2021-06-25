package com.idera.sqlcm.ui.instancedetails;

import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEvent;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMTreeNode;
import com.idera.sqlcm.entities.StatisticData;
import com.idera.sqlcm.ui.BaseViewModel;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;

// TODO IR just for testing
public class STABS__ {

    private STABS__() {}

    private static SimpleDateFormat formatter = new SimpleDateFormat("dd-MM-yyyy HH:mm:ss");

    private static StatisticData.Statistic createStatics(String category, String dateStr, int count) {
        StatisticData.Statistic s = new StatisticData.Statistic();
        try {
            s.setDate(formatter.parse(dateStr));
        } catch (ParseException e) {
            throw new RuntimeException(e);
        }
        s.setCount(count);
        s.setCategoryName(category);
        return s;
    }

    public static List<StatisticData.Statistic> getInstanceStatsData(long instId,
                                                     BaseViewModel.Interval days, BaseViewModel.Category categoryId) {
        List<StatisticData.Statistic> list = new ArrayList<>();
        String category = "Actual";
        switch (days) {
            case ONE_DAY: // each hour
                list.add(createStatics(category, "01-01-2015 00:00:00", 0));
                list.add(createStatics(category, "01-01-2015 01:00:00", 0));
                list.add(createStatics(category, "01-01-2015 02:00:00", 101));
                list.add(createStatics(category, "01-01-2015 03:00:00", 0));
                list.add(createStatics(category, "01-01-2015 04:00:00", 35));
                list.add(createStatics(category, "01-01-2015 05:00:00", 67));
                list.add(createStatics(category, "01-01-2015 06:00:00", 0));
                list.add(createStatics(category, "01-01-2015 07:00:00", 20));
                list.add(createStatics(category, "01-01-2015 08:00:00", 25));
                list.add(createStatics(category, "01-01-2015 09:00:00", 30));
                list.add(createStatics(category, "01-01-2015 10:00:00", 12));
                list.add(createStatics(category, "01-01-2015 11:00:00", 23));
                list.add(createStatics(category, "01-01-2015 12:00:00", 0));
                list.add(createStatics(category, "01-01-2015 13:00:00", 24));
                list.add(createStatics(category, "01-01-2015 14:00:00", 0));
                list.add(createStatics(category, "01-01-2015 15:00:00", 0));
                list.add(createStatics(category, "01-01-2015 16:00:00", 23));
                list.add(createStatics(category, "01-01-2015 17:00:00", 0));
                list.add(createStatics(category, "01-01-2015 18:00:00", 0));
                list.add(createStatics(category, "01-01-2015 19:00:00", 0));
                list.add(createStatics(category, "01-01-2015 20:00:00", 45));
                list.add(createStatics(category, "01-01-2015 21:00:00", 0));
                list.add(createStatics(category, "01-01-2015 22:00:00", 0));
                list.add(createStatics(category, "01-01-2015 23:00:00", 7));
                list.add(createStatics(category, "02-01-2015 00:00:00", 0));
                break;
            case SEVEN_DAY: // 6 hours
                list.add(createStatics(category, "01-01-2015 00:00:00", 0));
                list.add(createStatics(category, "01-01-2015 06:00:00", 35));
                list.add(createStatics(category, "01-01-2015 12:00:00", 45));
                list.add(createStatics(category, "01-01-2015 18:00:00", 0));

                list.add(createStatics(category, "02-01-2015 00:00:00", 0));
                list.add(createStatics(category, "02-01-2015 06:00:00", 15));
                list.add(createStatics(category, "02-01-2015 12:00:00", 0));
                list.add(createStatics(category, "02-01-2015 18:00:00", 34));

                list.add(createStatics(category, "03-01-2015 00:00:00", 0));
                list.add(createStatics(category, "03-01-2015 06:00:00", 30));
                list.add(createStatics(category, "03-01-2015 12:00:00", 14));
                list.add(createStatics(category, "03-01-2015 18:00:00", 38));

                list.add(createStatics(category, "04-01-2015 00:00:00", 33));
                list.add(createStatics(category, "04-01-2015 06:00:00", 34));
                list.add(createStatics(category, "04-01-2015 12:00:00", 35));
                list.add(createStatics(category, "04-01-2015 18:00:00", 0));

                list.add(createStatics(category, "05-01-2015 00:00:00", 0));
                list.add(createStatics(category, "05-01-2015 06:00:00", 35));
                list.add(createStatics(category, "05-01-2015 12:00:00", 0));
                list.add(createStatics(category, "05-01-2015 18:00:00", 0));

                list.add(createStatics(category, "06-01-2015 00:00:00", 0));
                list.add(createStatics(category, "06-01-2015 06:00:00", 35));
                list.add(createStatics(category, "06-01-2015 12:00:00", 0));
                list.add(createStatics(category, "06-01-2015 18:00:00", 0));

                list.add(createStatics(category, "07-01-2015 00:00:00", 0));
                list.add(createStatics(category, "07-01-2015 06:00:00", 35));
                list.add(createStatics(category, "07-01-2015 12:00:00", 0));
                list.add(createStatics(category, "07-01-2015 18:00:00", 0));
                break;
            case THIRTY_DAY: // each day
                list.add(createStatics(category, "01-01-2015 00:00:00", 0));
                list.add(createStatics(category, "02-01-2015 00:00:00", 25));
                list.add(createStatics(category, "03-01-2015 00:00:00", 0));
                list.add(createStatics(category, "04-01-2015 00:00:00", 45));
                list.add(createStatics(category, "05-01-2015 00:00:00", 10));
                list.add(createStatics(category, "06-01-2015 00:00:00", 23));
                list.add(createStatics(category, "07-01-2015 00:00:00", 25));
                list.add(createStatics(category, "08-01-2015 00:00:00", 0));
                list.add(createStatics(category, "09-01-2015 00:00:00", 25));
                list.add(createStatics(category, "10-01-2015 00:00:00", 23));
                list.add(createStatics(category, "11-01-2015 00:00:00", 0));
                list.add(createStatics(category, "12-01-2015 00:00:00", 25));
                list.add(createStatics(category, "13-01-2015 00:00:00", 0));
                list.add(createStatics(category, "14-01-2015 00:00:00", 45));
                list.add(createStatics(category, "15-01-2015 00:00:00", 33));
                list.add(createStatics(category, "16-01-2015 00:00:00", 0));
                list.add(createStatics(category, "17-01-2015 00:00:00", 25));
                list.add(createStatics(category, "18-01-2015 00:00:00", 0));
                list.add(createStatics(category, "19-01-2015 00:00:00", 45));
                list.add(createStatics(category, "20-01-2015 00:00:00", 40));
                list.add(createStatics(category, "21-01-2015 00:00:00", 0));
                list.add(createStatics(category, "22-01-2015 00:00:00", 25));
                list.add(createStatics(category, "23-01-2015 00:00:00", 0));
                list.add(createStatics(category, "24-01-2015 00:00:00", 45));
                list.add(createStatics(category, "25-01-2015 00:00:00", 65));
                list.add(createStatics(category, "26-01-2015 00:00:00", 0));
                list.add(createStatics(category, "27-01-2015 00:00:00", 25));
                list.add(createStatics(category, "28-01-2015 00:00:00", 0));
                list.add(createStatics(category, "29-01-2015 00:00:00", 45));
                list.add(createStatics(category, "30-01-2015 00:00:00", 0));
                break;
        }
        return list;
    }

    public static List<CMEvent> getInstanceAuditEvents() {
        List<CMEvent> eventList = new ArrayList();
        for (int i = 1; i <= 15; ++i) {
            CMEvent e = new CMEvent();
            e.setId(i);
            e.setDatabase("Database name " + i);
            e.setEvent("Event " + i);
            e.setCategory("Category " + i);
            e.setTime(Calendar.getInstance().getTime());
            e.setLogin("Login " + i);
            eventList.add(e);
        }
        return eventList;
    }

    public static List<CMInstance> getInstancesStab() {
        List<CMInstance> instanceList = new ArrayList();
        for (int i = 1; i <= 15; ++i) {
            CMInstance ins = new CMInstance();
            ins.setId(i);
            ins.setInstanceName("INSTANCE NAME " + i);
            instanceList.add(ins);
        }
        return instanceList;
    }

    public static List<CMTreeNode> getDatabaseList() {
        List<CMTreeNode> list = new ArrayList<CMTreeNode>(5);

        for (int i = 1; i < 15; ++i) {
            CMTreeNode cmTreeNode = new CMTreeNode();
            cmTreeNode.setId(i);
            cmTreeNode.setParentId(5); // Instance id ???
            cmTreeNode.setName("Database name " + i);
            cmTreeNode.setType(CMEntity.NodeType.Database);
            list.add(cmTreeNode);
        }
        return list;
    }

}
