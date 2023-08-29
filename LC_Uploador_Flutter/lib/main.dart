import 'dart:io';

import 'package:excel/excel.dart';
import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:path/path.dart' as p;

void main() {
  runApp(const MyApp());
}

class LCData {
  String id = "";
  String fileName = "";
  int state = -1;
  String errorMessage = "";

  LCData(
      {required this.id,
      required this.fileName,
      required this.state,
      required this.errorMessage});
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flutter Demo',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
        useMaterial3: true,
      ),
      home: const MyHomePage(title: 'LC-Uploader：请务必做好备份工作后，在进行操作'),
    );
  }
}

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});

  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  List<LCData> lcData = [];
  var resourcePath = "";
  var message = "";

  resourceDataFileSelected() async {
    FilePickerResult? result = await FilePicker.platform.pickFiles(
      type: FileType.custom,
      allowedExtensions: ['xlsx'],
    );
    try {
      if (result != null) {
        lcData = [];
        var file = result.files.single.path!;
        var bytes = File(file).readAsBytesSync();
        var excel = Excel.decodeBytes(bytes);
        if (excel != null && excel.tables.isNotEmpty) {
          var table = excel.tables.values.first;
          var idIndex = -1;
          var rawFileNameIndex = -1;
          for (var i = 0; i < table.maxCols; i++) {
            var cell = table
                .cell(CellIndex.indexByColumnRow(columnIndex: i, rowIndex: 0));
            if (cell.value.toString() == "id") {
              idIndex = i;
            }
            if (cell.value.toString() == "resource_file_name") {
              rawFileNameIndex = i;
            }
          }
          if (idIndex != -1 && rawFileNameIndex != -1) {
            for (var i = 1; i < table.maxRows; i++) {
              var id = table
                  .cell(CellIndex.indexByColumnRow(
                      columnIndex: idIndex, rowIndex: i))
                  .value
                  .toString();
              var fileName = table
                  .cell(CellIndex.indexByColumnRow(
                      columnIndex: rawFileNameIndex, rowIndex: i))
                  .value
                  .toString();
              lcData.add(LCData(
                  id: id.trim().replaceAll("\"", ""),
                  fileName: fileName.trim().replaceAll("\"", ""),
                  state: -1,
                  errorMessage: ""));
            }
            setState(() {
              message = "数据表格加载结束";
            });
          } else {
            setState(() {
              message = "未找到 id 或 resource_file_name 列";
            });
          }
        }
      }
    } catch (e) {
      setState(() {
        message = "数据库文件打开失败：${e.toString()}";
      });
    }
  }

  startProcessing() {
    if (FileSystemEntity.isDirectorySync(resourcePath) && lcData.isNotEmpty) {
      setState(() {
        message = "开始处理，请勿操作...";
      });
      Future.delayed(const Duration(seconds: 5), () {
        lcData.removeWhere((element) => element.id == "-1");
        for (var lc in lcData) {
          lc.state = -1;
        }
        for (var file in Directory(resourcePath).listSync()) {
          var fileNameWithoutExt = p.basenameWithoutExtension(file.path);
          var lc = lcData.firstWhere(
              (t) => fileNameWithoutExt.contains(t.fileName), orElse: () {
            return LCData(
                id: "-1",
                fileName: fileNameWithoutExt,
                state: 0,
                errorMessage: "文件存在，但数据表格中未找到");
          });
          if (lc.id == "-1") {
            lcData.add(lc);
          } else {
            try {
              file.renameSync(p.join(p.dirname(file.path), lc.id + p.extension(file.path)));
              lc.state = 1;
              lc.errorMessage = "";
            } catch (e) {
              lc.state = 0;
              lc.errorMessage = "文件名转换失败 ${e.toString()}";
            }
          }
        }
        for (var lc in lcData.where((element) => element.state == -1)) {
          lc.state = 0;
          lc.errorMessage = "未在文件夹中找到该文件";
        }
        setState(() {
          message = "处理结束";
        });
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
          //backgroundColor: Theme.of(context).colorScheme.primary,
          title: Text(widget.title),
        ),
        body: Padding(
          padding: const EdgeInsets.all(10.0),
          child: Column(
            children: [
              Container(
                height: 50,
                color: Colors.white70,
                child: Row(
                  children: [
                    ElevatedButton(
                        onPressed: () async {
                          String? selectedDirectory =
                              await FilePicker.platform.getDirectoryPath();

                          if (selectedDirectory != null) {
                            setState(() {
                              resourcePath = selectedDirectory!;
                            });
                          }
                        },
                        child: Text('选择资源所在文件夹')),
                    SizedBox(
                      width: 10,
                    ),
                    Expanded(child: Text(resourcePath)),
                  ],
                ),
              ),
              Container(
                height: 50,
                child: Row(
                  children: [
                    ElevatedButton(
                        onPressed: resourceDataFileSelected,
                        child: Text('加载资源数据库文件')),
                    Expanded(child: Container())
                  ],
                ),
              ),
              SizedBox(
                height: 20,
              ),
              Expanded(
                  child: Container(
                alignment: Alignment.topLeft,
                child: ListView.builder(
                  itemCount: lcData.length,
                  itemBuilder: (BuildContext context, int index) {
                    return ListTile(
                      title: Text("原始文件名：${lcData[index].fileName}"),
                      subtitle: Container(
                        alignment: Alignment.topLeft,
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text("数据库编号：${lcData[index].id}"),
                            Text(lcData[index].state == 0
                                ? "错误信息: ${lcData[index].errorMessage}"
                                : "")
                          ],
                        ),
                      ),
                      leading: lcData[index].state == -1
                          ? Icon(Icons.question_mark)
                          : (lcData[index].state == 0
                              ? Icon(Icons.error)
                              : Icon(Icons.check)),
                    );
                  },
                ),
              )),
              SizedBox(
                height: 20,
              ),
              Container(
                height: 50,
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    ElevatedButton(
                        onPressed: startProcessing, child: Text('开始')),
                    SizedBox(
                      width: 10,
                    ),
                    Expanded(
                      child: Text(message),
                    )
                  ],
                ),
              )
            ],
          ),
        ));
  }
}
