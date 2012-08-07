#pragma once
#include <iostream>
#include <fstream>
#include <iomanip>
#include <list>
#include <map>
#include <queue>

#include "Node.h"

using namespace std;

class CTree
{
private :
	int mBegin; //시작 노드 id
	int mEnd; //목적 노드 id
	int* mGasStation; //주유소 배열. index:id , value:기름값
	int mGasCount; //주유소 개수
	int** mAdjacencyMatrix; //각 노드간 거리에 대한 인접행렬
	int mLineCount; // 간선(길) 수 

	CNode* mHeadNode; //시작 노드
	CNode* mCurrentNode;
	list<int> mListMinimumLoad; //최선의 길 저장
	queue<int> mQueueWorkedLoad; //노드를 입력된 순서대로 처리
	map<int, CNode*> mMapPassNode;
	int mCurrentDepth;

	void Insert(int id); //주유소 id를 통해 객체 넣기

public:
	CTree(void);
	~CTree(void);
	void InitWithTxt(); //텍스트 파일을 읽어 주유소 및 길 설정
	void DisplayMatrix(); //InitWithTxt()를 통해 얻어온 정보로 인접 행렬 생성
	void Start();
};

