#include "Tree.h"

CTree::CTree(void)
{
	this->mBegin = 0;
	this->mEnd = 0;
	this->mGasStation = NULL;
	this->mGasCount = 0;
	this->mAdjacencyMatrix = NULL;
	this->mLineCount = 0;

	this->mHeadNode = NULL;
	this->mCurrentNode = NULL;
	this->mListMinimumLoad.clear();
	this->mMapPassNode.clear();
	this->mCurrentDepth = 0;
}

CTree::~CTree(void)
{
}

void CTree::InitWithTxt()
{
	ifstream fin;
	fin.open( "input.txt" );

	//ó���� �� ���� ��� id ���ϱ�
	fin >> this->mBegin; //���� ���� id
	fin >> this->mEnd; //���� ���� id

	//������ ����
	fin >> this->mGasCount; //������ ����
	this->mGasStation = new int[this->mGasCount];

	int id=0;
	for( int i=0; i<this->mGasCount; i++ )
	{
		fin >> id; //������ id
		if( id == i) //�������� ������ id �Է°�����, ������ �´����� ����
			fin >> this->mGasStation[i];
		else
		{
			cout << "input ���� ���� ���� " << endl;
			exit(1);
		}
	}

	//�� �������� ��� ����ġ(����)�� �����ϱ� ���Ͽ� ������� �����
	fin >> this->mLineCount; //�� ����
	
	this->mAdjacencyMatrix = new int*[this->mLineCount];
	for( int i=0; i<this->mLineCount; i++)
	{
		this->mAdjacencyMatrix[i] = new int[this->mLineCount];
	}

	for( int i=0; i<this->mLineCount; i++ )
	{
		for( int j=0; j<this->mLineCount; j++ )
		{
			this->mAdjacencyMatrix[i][j] = -1; //�ڱ� �ڽ��̰ų� , ���� ���� �濡�� -1��
		}
	}

	int row = 0, col = 0; //��, ��
	int dis = 0; //�Ÿ�
	for( int i=0; i<this->mLineCount; i++)
	{
		fin >> row;
		fin >> col;
		fin >> dis;

		this->mAdjacencyMatrix[row][col] = dis;
	}

	fin.close();
}

void CTree::DisplayMatrix()
{
	for( int i=0; i<this->mLineCount; i++ )
	{
		for( int j=0; j<this->mLineCount; j++ )
		{
			cout  << setw(5) << this->mAdjacencyMatrix[i][j];
		}
		cout << endl;
	}
}

void CTree::Start()
{
	int id = 0;
	while( id != mEnd )
	{
		if( this->mQueueWorkedLoad.empty() ) //ó���� ��尡 ���� ��
		{
			Insert( mBegin ); //ó�� ��� ����
			break;
		}
		//ù�� ° ����( ���� ��� �α��� ����ġ�� 0�̱� ���� )
		//����ġ : �������� �Ÿ� * ���� ������ �⸧��
		//ex) 0->1 - 9km, 10km �⸧�� ������ �ֱ� ������ 9km�� �׳� �� �� �ִ� => ����ġ 0
		//ex) 1->2 - 5km, 5km�� ���� ���ؼ� �߰��� 4km �ʿ� => ����ġ : 4km * 1�� ������ �⸧��
		else if( this->mCurrentDepth == 1 ) 
		{
		}
	}
}

void CTree::Insert(int id)
{
	CNode* temp = new CNode();
	temp->mID = id; //id
	temp->mPrice = this->mGasStation[id];
	
	if( this->mHeadNode == NULL )//Head�� ����� ��, ���� ù ���
	{
		temp->mTotalPrice = 0; //ó���̴ϱ� ����ġ�� 0
		temp->mDepth = 0; //ó�� ����
		temp->mOil = 10; //ó�� �����ҿ��� ������ �ִ� �⸧�� 10L
		temp->mSpendOil = 0;

		temp->mPreNode = NULL; //ó�� ����̴ϱ� �ڿ� ���� ����.

		int distance = 0;
		//������ �ѹ�
		for( int i=0; i<this->mGasCount; i++ )
		{
			distance = this->mAdjacencyMatrix[id][i];
			if( distance != -1 && distance != id ) //������ ��� �ڱ� �ڽ��� �ƴ� �� �����Ѱ���
			{
				CNode* apTemp = new CNode();
				apTemp->mID = i;
				apTemp->mPrice = this->mGasStation[i]; 
				if( temp->mOil - distance >= 0 ) //�����ҷ� ���� �濡 �⸧�� ���Ұų� �� ���� ��
				{
					apTemp->mTotalPrice = 0; //�⸧�� ���ұ� ������ �⸧�� �ȵ���
					apTemp->mSpendOil = 0;
					apTemp->mOil = temp->mOil - distance; //���� �����ҿ��� ���� ������ �Ÿ���ŭ ���������
				}
				else //�⸧���� ����� ��
				{
					apTemp->mTotalPrice = (-(temp->mOil-distance)) * temp->mPrice; //���ڶ� �⸧ * ���� ������ �⸧��
					apTemp->mSpendOil = -(temp->mOil-distance); //�����ҿ��� �⸧ ä�� ��
					apTemp->mOil = 0; //���� �����ҿ��� ���� �����Ҹ�ŭ�� �⸧�� ������ �־����� ����� 0
				}
				apTemp->mDepth = temp->mDepth+1; //���� ������ ���� ���� +1
				apTemp->mPreNode = temp; //���� ���� ���� �����Ҹ� ����Ų��.

				temp->mListApproachNode.push_back(apTemp); //���� ����� ���� ��忡 �߰�

				this->mQueueWorkedLoad.push(apTemp->mID); //���� ���� �۾����̴ϱ�
				this->mMapPassNode[apTemp->mID] = apTemp; //�ѹ��̶�� ��ģ ��带 ����
			}
		}
		distance = 0;
		//������ �� �ѹ�
		for( int i=0; i<this->mGasCount; i++ )
		{
			distance = this->mAdjacencyMatrix[i][id];
			if( distance != -1 && distance != id ) //������ ��� �ڱ� �ڽ��� �ƴ� �� �����Ѱ���
			{
				CNode* apTemp = new CNode();
				apTemp->mID = i;
				apTemp->mPrice = this->mGasStation[i]; 
				if( temp->mOil - distance >= 0 ) //�����ҷ� ���� �濡 �⸧�� ���Ұų� �� ���� ��
				{
					apTemp->mTotalPrice = 0; //�⸧�� ���ұ� ������ �⸧�� �ȵ���
					apTemp->mSpendOil = 0;
					apTemp->mOil = temp->mOil - distance; //���� �����ҿ��� ���� ������ �Ÿ���ŭ ���������
				}
				else //�⸧���� ����� ��
				{
					apTemp->mTotalPrice = (-(temp->mOil-distance)) * temp->mPrice; //���ڶ� �⸧ * ���� ������ �⸧��
					apTemp->mSpendOil = -(temp->mOil-distance); //�����ҿ��� �⸧ ä�� ��
					apTemp->mOil = 0; //���� �����ҿ��� ���� �����Ҹ�ŭ�� �⸧�� ������ �־����� ����� 0
				}
				apTemp->mDepth = temp->mDepth+1; //���� ������ ���� ���� +1
				apTemp->mPreNode = temp; //���� ���� ���� �����Ҹ� ����Ų��.
				
				temp->mListApproachNode.push_back(apTemp); //���� ����� ���� ��忡 �߰�

				this->mQueueWorkedLoad.push(apTemp->mID); //���� ���� �۾����̴ϱ�
				this->mMapPassNode[apTemp->mID] = apTemp; //�ѹ��̶�� ��ģ ��带 ����
			}
		}

		this->mHeadNode = temp; //���� ���� ��� �Ҵ�
		mMapPassNode[id] = temp; //������ ������ ����.
		mListMinimumLoad.push_back(id); //�ּ��� �濡 ����.
	}
	else
	{

	}
}