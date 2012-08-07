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
			cout << "�˼��մϴ�. 10�ʰ� �Ѿ��ų� �޸� �����Դϴ�.. " << endl;
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
	tree.DisplayMatrix(); //������� ó�� ���� �� ���
	tree.Start();
	//tree.DisplayMatrix(); //������� ������ ���� �� ã�� ����
	tree.Result();
	threadResult = false;
	cout << "�ɸ� �ð� : " << t2 - t1 << "������ -���� ����� �ʹ� ����.." << endl;
	return 0;
}