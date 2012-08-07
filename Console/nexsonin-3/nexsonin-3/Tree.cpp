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
	this->mListPassNode.clear();
	this->mCurrentDepth = 0;
}

CTree::~CTree(void)
{
}

void CTree::InitWithTxt()
{
	ifstream fin;
	fin.open( "input.txt" );

	//처음과 끝 지점 노드 id 구하기
	fin >> this->mBegin; //시작 지점 id
	fin >> this->mEnd; //목적 지점 id

	//주유소 관련
	fin >> this->mGasCount; //주유소 개수
	this->mGasStation = new int[this->mGasCount];

	int id=0;
	for( int i=0; i<this->mGasCount; i++ )
	{
		fin >> id; //주유소 idrmrj
		if( id == i) //순차적인 주유소 id 입력값으로, 형식이 맞는지에 대해
			fin >> this->mGasStation[i];
		else
		{
			cout << "input 파일 형식 오류 " << endl;
			exit(1);
		}
	}

	//각 지점간의 길과 가중치(길이)를 관리하기 위하여 인접행렬 만들기
	fin >> this->mLineCount; //길 개수
	
	this->mAdjacencyMatrix = new int*[this->mLineCount];
	for( int i=0; i<this->mLineCount; i++)
	{
		this->mAdjacencyMatrix[i] = new int[this->mLineCount];
	}

	for( int i=0; i<this->mLineCount; i++ )
	{
		for( int j=0; j<this->mLineCount; j++ )
		{
			this->mAdjacencyMatrix[i][j] = -1; //자기 자신이거나 , 갈수 없는 길에는 -1로
		}
	}

	int row = 0, col = 0; //행, 열
	int dis = 0; //거리
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
		if( this->mHeadNode == NULL) //처리할 노드가 없을 때
		{
			Insert( mBegin ); //처음 노드 연결
			id = this->mQueueWorkLoad.front();
			this->mQueueWorkLoad.pop();
		}
		//첫번 째 깊이( 시작 노드 인근은 가중치가 0이기 때문 )
		//가중치 : 가기위한 거리 * 현재 주유소 기름값
		//ex) 0->1 - 9km, 10km 기름을 가지고 있기 때문에 9km는 그냥 갈 수 있다 => 가중치 0
		//ex) 1->2 - 5km, 5km을 가기 위해선 추가로 4km 필요 => 가중치 : 4km * 1번 주유소 기름값
		else 
		{
			Insert(id);
			id = this->mQueueWorkLoad.front();
			this->mQueueWorkLoad.pop();
		}
	}
}

void CTree::Insert(int id)
{
	CNode* temp = NULL;
	if( this->mHeadNode == NULL )//Head가 비었을 때, 제일 첫 노드
	{
		temp = new CNode();
		temp->mID = id; //id
		temp->mPrice = this->mGasStation[id];

		temp->mTotalPrice = 0; //처음이니까 가중치는 0
		temp->mDepth = 0; //처음 깊이
		temp->mOil = 10; //처음 주유소에서 가지고 있는 기름은 10L
		temp->mSpendOil = 0;

		temp->mPreNode = NULL; //처음 노드이니까 뒤에 노드는 없음.

		this->mHeadNode = temp; //시작 지점 노드 할당
		mListPassNode.push_back(temp); //지나간 주유소 저장.
		mListMinimumLoad.push_back(id); //최선의 길에 저장.
	}
	else
	{
		for( this->mIterNode=this->mListPassNode.begin(); mIterNode!=this->mListPassNode.end(); this->mIterNode++)
		{
			if( (*this->mIterNode)->mID == id )
				break;
		}
		temp = *this->mIterNode; //현재 처리해야할 노드 가져오기
	}
	int distance = 0;
	//행으로 한번
	for( int i=0; i<this->mGasCount; i++ )
	{
		distance = this->mAdjacencyMatrix[id][i];
		if( distance != -1 && distance != id )
		{
			ApproachNodeInsert(distance, temp, i);
		}
	}
	distance = 0;
	//열으로 또 한번
	for( int i=0; i<this->mGasCount; i++ )
	{
		distance = this->mAdjacencyMatrix[i][id];if( distance != -1 && distance != id )
		{
			ApproachNodeInsert(distance, temp, i);
		}
	}
}

void CTree::ApproachNodeInsert(int distance, CNode* preNode, int nowID)
{
	CNode* apTemp = new CNode();
	apTemp->mID = nowID;
	apTemp->mPrice = this->mGasStation[nowID]; 
	if( preNode->mOil - distance >= 0 ) //주유소로 가는 길에 기름이 남았거나 딱 맞을 때
	{
		apTemp->mTotalPrice = preNode->mTotalPrice; //기름이 남았기 때문에 기름값 안들음
		apTemp->mSpendOil = 0;
		apTemp->mOil = preNode->mOil - distance; //이전 주유소에서 지금 주유소 거리만큼 사용했으니
	}
	else //기름값이 들었을 때
	{
		apTemp->mSpendOil =  -(preNode->mOil-distance); //주유소에서 기름 채운 양
		apTemp->mTotalPrice = preNode->mTotalPrice + apTemp->mSpendOil * preNode->mPrice; //모자란 기름 * 이전 주유소 기름값
		apTemp->mOil = 0; //이전 주유소에서 지금 주유소만큼의 기름만 가지고 있었으니 현재는 0
	}
	apTemp->mDepth = preNode->mDepth+1; //이전 주유소 깊이 보다 +1
	apTemp->mPreNode = preNode; //이전 노드는 이전 주유소를 가리킨다.
	
	for( this->mIterNode=this->mListPassNode.begin(); mIterNode!=this->mListPassNode.end(); this->mIterNode++)
	{
		if( (*this->mIterNode)->mID == nowID )
			break;
	}
	
	if( this->mIterNode != this->mListPassNode.end() ) //이전에 처리했던 node라면
	{
		if( (*this->mIterNode)->mTotalPrice > apTemp->mTotalPrice ) //거쳐갔던 노드 가중치가 더 크면 바꾼다.
		{
			CNode* removeNode = *this->mIterNode;
			removeNode->mPreNode->mListApproachNode.remove(removeNode); //이전 노드의 인접노드들 중에서 해당 노드 삭제

			delete removeNode;
			removeNode = NULL;

			this->mListPassNode.remove( *this->mIterNode );

			//삭제한 node의 id를 삭제하기 위하여 다시 큐를 만들어....
			queue<int> temp;
			while( !this->mQueueWorkLoad.empty() )
			{
				int id = this->mQueueWorkLoad.front();
				this->mQueueWorkLoad.pop();
				if( id == nowID )
				{
					continue;
				}
				else if( !mExtraApproachNode.empty() )
				{
					list<int>::iterator iter;
					for( iter = mExtraApproachNode.begin(); iter!=mExtraApproachNode.end(); iter++ )
					{
						for( this->mIterNode=this->mListPassNode.begin(); mIterNode!=this->mListPassNode.end(); this->mIterNode++)
						{
							if( (*this->mIterNode)->mID == *iter )
							{
								this->mListPassNode.remove(*this->mIterNode);
								break;
							}
						}

					}
					continue;
				}
				else
					temp.push( id );
			}
			this->mQueueWorkLoad = temp;
		}
		else
		{ //이전 노드가 더 최소비용이라면 아무것도 안하고 끝
			delete apTemp;
			apTemp = NULL;
			return ;
		}
	}
	preNode->mListApproachNode.push_back(apTemp); //이전 노드의 인접 노드에 추가

	this->mQueueWorkLoad.push(apTemp->mID); //현재 노드는 다음에 처리해야 할 노드
	this->mListPassNode.push_back(apTemp);//한번이라고 거친 노드를 저장
	//this->mListPassNode[apTemp->mID] = apTemp; 
}

