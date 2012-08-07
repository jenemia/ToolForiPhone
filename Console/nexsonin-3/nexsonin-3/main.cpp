#include <iostream>
#include <string>
#include <Windows.h>
#include <time.h>

#include "Tree.h"

using namespace std;

clock_t t1, t2;
bool threadResult = true;
DWORD WINAPI Thread( void* arg )
{
	while( threadResult )
	{
		t2 = clock();
		if( t2 - t1 > 1000 )
		{
			cout << "죄송합니다. 10초가 넘었거나 메모리 오류입니다.. " << endl;
			exit(1);
		}
	}
	
	return 0;
}

int main()
{
	DWORD dwThreadID;
	t1 = clock();
	CreateThread( NULL, 0, Thread, NULL, 0, &dwThreadID );

	CTree tree;
	tree.InitWithTxt();
	tree.DisplayMatrix(); //인접행렬 처음 생성 후 출력
	tree.Start();
	//tree.DisplayMatrix(); //인접행렬 마지막 최적 길 찾기 위한
	tree.Result();
	threadResult = false;
	cout << "걸린 시간 : " << t2 - t1 << "가끔씩 -값은 계산이 너무 빨라서.." << endl;
	return 0;
}